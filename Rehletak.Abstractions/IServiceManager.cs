using Rehletak.Abstractions.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rehletak.Abstractions
{
    public interface IServiceManager
    {
        IAuthService authService { get; }
    }
}
