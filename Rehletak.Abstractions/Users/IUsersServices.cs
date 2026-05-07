using Rehletak.Shared.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rehletak.Abstractions.Users
{
    public interface IUsersServices
    {
        Task<UserResponse> getCurrentUserAsync(string userId);
        Task<UserResponse> updateUserAsync(string userId, UpdateUserRequest request);
    }
}
