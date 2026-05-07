using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Rehletak.Abstractions;
using Rehletak.Abstractions.Auth;
using Rehletak.Abstractions.Users;
using Rehletak.Domain.Entites.Auth;
using Rehletak.Presistense.Contexts;
using Rehletak.Services.Auth.Options;
using Rehletak.Services.Users;
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
        RoleManager<IdentityRole> roleManager,
        IOptionsSnapshot<JwtOptions> jwtOptions,
        IOptionsSnapshot<TwilioOption> twilioOptions,
        IOptionsSnapshot<EmailSettings> emailSettings
        ) : IServiceManager
    {
        public IAuthService authService { get; } = new AuthService(twilioOptions,jwtOptions,emailSettings,connectionMultiplexer,context,userManager, roleManager);

        public IUsersServices usersServices { get;  } = new UsersServices(userManager, roleManager);
    }
}
