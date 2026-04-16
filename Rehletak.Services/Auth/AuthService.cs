using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Rehletak.Domain.Entites.Auth;
using Rehletak.Domain.Exceptions.UnAuthorize;
using Rehletak.Presistense.Contexts;
using Rehletak.Services.Auth.Options;
using Rehletak.Shared.Dtos.Auth;
using Rehletak.Shared.Dtos.Auth.Login;
using Rehletak.Shared.Dtos.Auth.RegisterByEmail;
using Rehletak.Shared.Dtos.Auth.ResetPassword;
using Rehletak.Shared.Dtos.Auth.SendOtp;
using Rehletak.Shared.Dtos.Auth.SmsOtp;
using StackExchange.Redis;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Rehletak.Abstractions.Auth
{
    public class AuthService(
        IOptionsSnapshot<TwilioOption> twilioOptions,
        IOptionsSnapshot<JwtOptions> jwtOptions,
        IOptionsSnapshot<EmailSettings> emailSettings,
        IConnectionMultiplexer redis,
        RehletakDbContext context,
        UserManager<AppUser> userManager
        ) : IAuthService
    {

        private readonly IDatabase db = redis.GetDatabase();

        public async Task<SendOtpResponse> SendOtpAsync(SendOtpRequest request)
        {
            // Generate OTP
            var otp = new Random().Next(100000, 999999).ToString();

            // Save OTP with 120 sec expiry
            await db.StringSetAsync(request.phoneNumper, otp, TimeSpan.FromMinutes(2));

            // Init Twilio
            var accountSid = twilioOptions.Value.AccountSID;
            var authToken = twilioOptions.Value.AuthToken;
            TwilioClient.Init(accountSid, authToken);

            // Send SMS
            await MessageResource.CreateAsync(
                body: $"Your OTP code is: {otp}. Valid for 2 minutes.",
                from: new PhoneNumber(twilioOptions.Value.PhoneNumber),
                to: new PhoneNumber(request.phoneNumper)
            );

            return new SendOtpResponse
            {
                message = "OTP sent successfully",
                expiers_in = 120
            };

        }

        public async Task<VerifyOtpResponse> VerifyOtpAsync(VerifyOtpRequest request)
        {

            var storedOtp = await db.StringGetAsync(request.phoneNumper);

            if (storedOtp.HasValue && storedOtp == request.otp)
            {

                await db.KeyDeleteAsync(request.phoneNumper);

                // Here you can check if the user is new or existing based on your database

                var user = await context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.phoneNumper);


                if (user is not null)
                {
                    var accessToken = await GenerateJwtAccessTokenAsync(user);

                    // old user
                    return new VerifyOtpResponseOldUser
                    {
                        is_new_user = false,
                        accessToken = accessToken,
                        refreshToken = "dummy_refresh",
                        user = new UserResponse
                        {
                            id = user.Id,
                            userName = user.UserName,
                            role = userManager.GetRolesAsync(user).Result.FirstOrDefault(),
                            token = ""
                        }
                    };
                }
                else
                {
                    // new user
                    return new VerifyOtpResponseNewUser
                    {
                        is_new_user = false,
                        tempToken = generateJwttemptoken(request.phoneNumper)
                    };
                }

            }

            throw new UnAuthorizeException("Invalid or expired OTP");

        }

        public async Task<RegisterByEmailResponse> RegisterByEmailAsync(RegisterByEmailRequest request)
        {

            // Check if user already exists with this email
            var existingUser = await userManager.FindByEmailAsync(request.email);
            if (existingUser is not null)
            {
                throw new UnAuthorizeException("User with this email already exists");
            }

            // Generate OTP
            var otp = new Random().Next(100000, 999999).ToString();

            // Send OTP to email
            await SendOtpEmailAsync(request.email, otp);

            // Hash the password
            var hashedPassword = HashPassword(request.password);

            // Store OTP in Redis with email as key (expires in 2 minutes)
            await db.StringSetAsync(request.email, otp, TimeSpan.FromMinutes(2));

            var userInfo = new RegisterByEmailRequest
            {
                fullName = request.fullName,
                phoneNumber = request.phoneNumber,
                email = request.email,
                password = hashedPassword
            };

            // Store hashed password in Redis with email+otp as key (expires in 2 minutes)
            await db.StringSetAsync($"{request.email}:{otp}", JsonSerializer.Serialize(userInfo), TimeSpan.FromMinutes(2));

            return new RegisterByEmailResponse
            {
                message = "OTP sent successfully to your email"
            };

        }

        public async Task<VerifyEmailResponse> VerifyEmailAsync(VerifyEmailRequest request)
        {

            // Get OTP from Redis using email as key
            var storedOtp = await db.StringGetAsync(request.email);

            if (!storedOtp.HasValue || storedOtp != request.otp)
            {
                throw new UnAuthorizeException("Invalid or expired OTP");
            }

            // Get hashed password from Redis using email:otp as key
            var userInfo = await db.StringGetAsync($"{request.email}:{request.otp}");

            if (!userInfo.HasValue)
            {
                throw new UnAuthorizeException("Password not found. Please register again.");
            }

            var userInfoObj = JsonSerializer.Deserialize<RegisterByEmailRequest>(userInfo.ToString());


            if (userInfoObj is null)
            {
                throw new UnAuthorizeException("Invalid user info. Please register again.");
            }

            // Create new user
            var newUser = new AppUser
            {
                full_name = userInfoObj.fullName,
                UserName = userInfoObj.email,
                PhoneNumber = userInfoObj.phoneNumber,
                Email = userInfoObj.email
            };

            newUser.PasswordHash = userInfoObj.password;

            var result = await userManager.CreateAsync(newUser);

            if (!result.Succeeded)
            {
                throw new Exception($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            // Delete OTP and password from Redis
            await db.KeyDeleteAsync(request.email);
            await db.KeyDeleteAsync($"{request.email}:{request.otp}");

            // Generate temp token
            var accessToken = await GenerateJwtAccessTokenAsync(newUser);

            return new VerifyEmailResponse
            {
                accessToken = accessToken
            };


        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.email);

            if (user is null)
            {
                throw new UnAuthorizeException("Invalid email or password");
            }

            var passwordValid = await userManager.CheckPasswordAsync(user, request.password);

            if (!passwordValid)
            {
                throw new UnAuthorizeException("Invalid email or password");
            }

            var accessToken = await GenerateJwtAccessTokenAsync(user);

            var refreshToken = GenerateRefreshToken();

            var newRefreshToken = new RefreshToken
            {
                token = refreshToken,
                expires_at = DateTime.UtcNow.AddDays(7).ToLocalTime(),
                userId = user.Id,

            };

            context.refresh_tokens.Add(newRefreshToken);
            await context.SaveChangesAsync();

            return new LoginResponse
            {
                accessToken = accessToken,
                refreshToken = refreshToken
            };

        }

        public async Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.email);
            if (user is null) throw new UnAuthorizeException("User with this email does not exist");

            var otp = new Random().Next(100000, 999999).ToString();

            await SendOtpEmailAsync(request.email, otp);

            await db.StringSetAsync($"forget_password:{request.email}", otp, TimeSpan.FromMinutes(2));

            return new ForgotPasswordResponse
            {
                message = "Reset Password OTP has been sent to your email."
            };
        }

        public async Task<VerifyResetOtpResponse> VerifyResetOtpAsync(VerifyResetOtpRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.email);
            if (user is null) throw new UnAuthorizeException("User with this email does not exist");

            var storedOtp = await db.StringGetAsync($"forget_password:{request.email}");
            if (!storedOtp.HasValue || storedOtp != request.otp)
            {
                throw new UnAuthorizeException("Invalid or expired OTP");
            }

            await db.KeyDeleteAsync($"forget_password:{request.email}");

            return new VerifyResetOtpResponse
            {
                message = "OTP verified successfully. You can now reset your password."
            };
        }

        public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.email);
            if (user is null) throw new UnAuthorizeException("User with this email does not exist");

            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);

            var result = await userManager.ResetPasswordAsync(user, resetToken, request.newPassword);
            if (!result.Succeeded)
            {
                throw new Exception($"Failed to reset password: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            return new ResetPasswordResponse
            {
                message = "Password has been reset successfully."
            };
        }

        public async Task<LoginResponse> RefreshAsync(string oldRefreshtoken)
        {

            var oldToken = await context.refresh_tokens.Include(u => u.user).FirstOrDefaultAsync(t => t.token == oldRefreshtoken);

            if (!oldToken.isActive) throw new UnAuthorizeException("Invalid refresh token");

            var user = await userManager.FindByIdAsync(oldToken.userId);

            var accessToken = await GenerateJwtAccessTokenAsync(user);

            var newRefreshToken = GenerateRefreshToken();

            var refToken = new RefreshToken
            {
                token = newRefreshToken,
                expires_at = DateTime.UtcNow.AddDays(7).ToLocalTime(),
                userId = user.Id,
            };

            oldToken.revoked_at = DateTime.UtcNow;
            context.refresh_tokens.Add(refToken);
            await context.SaveChangesAsync();

            return new LoginResponse
            {
                accessToken = accessToken,
                refreshToken = newRefreshToken
            };

        }

        private async Task SendOtpEmailAsync(string email, string otp)
        {
            try
            {
                var settings = emailSettings.Value;

                using (var client = new SmtpClient(settings.Host, settings.Port))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(settings.Email, settings.Password);

                    var message = new MailMessage
                    {
                        From = new MailAddress(settings.Email),
                        Subject = "Your OTP Code",
                        Body = $"Your OTP code is: {otp}. Valid for 2 minutes.",
                        IsBodyHtml = false
                    };

                    message.To.Add(email);

                    await client.SendMailAsync(message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error sending email: {ex.Message}", ex);
            }
        }

        private async Task<string> GenerateJwtAccessTokenAsync(AppUser user)
        {
            var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtOptions.Value.Key)
        );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
            new Claim(ClaimTypes.NameIdentifier,user.Id),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.full_name),
            new Claim(ClaimTypes.MobilePhone, user.PhoneNumber)
        };

            var roles = await userManager.GetRolesAsync(user);

            if (roles is not null)
            {
                foreach (var item in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, item));
                }
            }

            var token = new JwtSecurityToken(
                issuer: jwtOptions.Value.Issuer,
                audience: jwtOptions.Value.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtOptions.Value.ExpireMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        private string generateJwttemptoken(string phoneNumber)
        {
            var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtOptions.Value.Key)
        );
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
            new Claim(ClaimTypes.MobilePhone, phoneNumber)
        };
            var token = new JwtSecurityToken(
                issuer: jwtOptions.Value.Issuer,
                audience: jwtOptions.Value.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string HashPassword(string password)
        {
            return userManager.PasswordHasher.HashPassword(null, password);
        }


    }

}
