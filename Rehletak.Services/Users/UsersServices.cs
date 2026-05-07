using Microsoft.AspNetCore.Identity;
using Rehletak.Abstractions.Users;
using Rehletak.Domain.Entites.Auth;
using Rehletak.Domain.Exceptions.UnAuthorize;
using Rehletak.Shared.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rehletak.Services.Users
{
    public class UsersServices(
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager
        ) : IUsersServices
    {
        public async Task<UserResponse> getCurrentUserAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new UnAuthorizeException("User not found");
            }

            var roles = await userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            return new UserResponse
            {
                id = user.Id,
                fullName = user.full_name,
                phoneNumber = user.PhoneNumber,
                email = user.Email,
                role = role
            };
        }

        public async Task<UserResponse> updateUserAsync(string userId, UpdateUserRequest request)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) throw new UnAuthorizeException("User not found");

            user.full_name = request.newFullName;
            user.Email = request.newEmail;
            user.PhoneNumber = request.newPhoneNumber;

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded) throw new Exception($"Failed to update user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            return new UserResponse
            {
                id = user.Id,
                fullName = user.full_name,
                phoneNumber = user.PhoneNumber,
                email = user.Email,
                role = (await userManager.GetRolesAsync(user)).FirstOrDefault()
            };
        }
    }
}
