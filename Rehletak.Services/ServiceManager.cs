using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Rehletak.Abstractions;
using Rehletak.Abstractions.Auth;
using Rehletak.Domain.Entites.Auth;
using Rehletak.Presistense.Contexts;
using Rehletak.Services.Auth.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rehletak.Services
{
    public class ServiceManager(
        IConnectionMultiplexer connectionMultiplexer,
        RehletakDbContext context,
        UserManager<AppUser> userManager,
        IOptionsSnapshot<JwtOptions> jwtOptions,
        IOptionsSnapshot<TwilioOption> twilioOptions,
        IOptionsSnapshot<EmailSettings> emailSettings
        ) : IServiceManager
    {
        public IAuthService authService { get; } = new AuthService(twilioOptions,jwtOptions,emailSettings,connectionMultiplexer,context,userManager);
    }
}
