using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rehletak.Abstractions;
using Rehletak.Shared.Dtos.Auth.Login;
using Rehletak.Shared.Dtos.Auth.RegisterByEmail;
using Rehletak.Shared.Dtos.Auth.ResetPassword;
using Rehletak.Shared.Dtos.Auth.SendOtp;
using Rehletak.Shared.Dtos.Auth.SmsOtp;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Rehletak.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IServiceManager serviceManager) : ControllerBase
    {
        [HttpPost("send-sms-otp")]
        public async Task<IActionResult> sendOtp(SendOtpRequest request)
        {
            var result = await serviceManager.authService.SendOtpAsync(request);

            return Ok(result);
        }

        [HttpPost("verify-sms-otp")]
        public async Task<IActionResult> verifyOtp(VerifyOtpRequest request)
        {
            var result = await serviceManager.authService.VerifyOtpAsync(request);

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> register(RegisterByEmailRequest request)
        {
            var result = await serviceManager.authService.RegisterByEmailAsync(request);

            return Ok(result);
        }

        [HttpPost("VerifyEmailOtp")]
        public async Task<IActionResult> varifyEmailOtp(VerifyEmailRequest request)
        {
            var result = await serviceManager.authService.VerifyEmailAsync(request);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> login(LoginRequest request)
        {
            var result = await serviceManager.authService.LoginAsync(request);

            return Ok(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> forgotPassword(ForgotPasswordRequest request)
        {
            var result = await serviceManager.authService.ForgotPasswordAsync(request);

            return Ok(result);
        }

        [HttpPost("verify-reset-otp")]
        public async Task<IActionResult> verifyResetOtp(VerifyResetOtpRequest request)
        {
            var result = await serviceManager.authService.VerifyResetOtpAsync(request);

            return Ok(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> resetPassword(ResetPasswordRequest request)
        {
            var result = await serviceManager.authService.ResetPasswordAsync(request);
            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> refreshToken(string oldRefreshToken)
        {

            var result = await serviceManager.authService.RefreshAsync(oldRefreshToken);
            return Ok(result);

        }


        //mmmmmmmmmmmm
    }
}
