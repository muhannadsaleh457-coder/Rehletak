using Rehletak.Abstractions.Auth;
using Rehletak.Abstractions.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rehletak.Abstractions
{
    public interface IServiceManager
    {
        IAuthService authService { get; }
        IUsersServices usersServices { get; }
    }
}
