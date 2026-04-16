using Rehletak.Shared.Dtos.Auth.Login;
using Rehletak.Shared.Dtos.Auth.RegisterByEmail;
using Rehletak.Shared.Dtos.Auth.ResetPassword;
using Rehletak.Shared.Dtos.Auth.SendOtp;
using Rehletak.Shared.Dtos.Auth.SmsOtp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rehletak.Abstractions.Auth
{
    public interface IAuthService
    {
        Task<SendOtpResponse> SendOtpAsync(SendOtpRequest request);
        Task<VerifyOtpResponse> VerifyOtpAsync(VerifyOtpRequest request);
        Task<RegisterByEmailResponse> RegisterByEmailAsync(RegisterByEmailRequest request);
        Task<VerifyEmailResponse> VerifyEmailAsync(VerifyEmailRequest request);
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordRequest request);
        Task<VerifyResetOtpResponse> VerifyResetOtpAsync(VerifyResetOtpRequest request);
        Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request);
        Task<LoginResponse> RefreshAsync (string oldRefreshtoken);
    }
}
