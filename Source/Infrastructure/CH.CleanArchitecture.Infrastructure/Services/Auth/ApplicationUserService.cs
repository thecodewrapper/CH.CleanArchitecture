using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Core.Application.DTOs;
using CH.CleanArchitecture.Core.Domain;
using CH.CleanArchitecture.Core.Domain.Entities.UserAggregate;
using CH.CleanArchitecture.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CH.CleanArchitecture.Infrastructure.Services
{
    ///<inheritdoc cref="IApplicationUserService"/>
    internal class ApplicationUserService : IApplicationUserService
    {
        private readonly ILogger<ApplicationUserService> _logger;
        private readonly IMapper _mapper;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILocalizationService _localizer;

        public ApplicationUserService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<ApplicationUserService> logger, IMapper mapper, ILocalizationService localizer) {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result> CreateUserAsync(User user, string password, List<string> roles, bool isActive) {
            var serviceResult = new Result().Successful();
            try {
                var applicationUser = _mapper.Map<ApplicationUser>(user);
                applicationUser.Id = Guid.NewGuid().ToString();
                applicationUser.IsActive = isActive;
                applicationUser.MustChangePassword = false;
                var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                using (transaction) {
                    var identityResult = await _userManager.CreateAsync(applicationUser, password);

                    if (!identityResult.Succeeded) {
                        serviceResult.Failed().WithErrors(_mapper.Map<List<ResultError>>(identityResult.Errors));
                    }
                    else if (roles?.Any() ?? false) {
                        var rolesResult = await _userManager.AddToRolesAsync(applicationUser, roles.Select(nr => nr.ToUpper()));
                        if (!rolesResult.Succeeded)
                            serviceResult.Failed().WithErrors(_mapper.Map<List<ResultError>>(identityResult.Errors));
                    }
                    else
                        serviceResult.Successful();

                    transaction.Complete();
                }
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to create user.");
            }
            return serviceResult;
        }

        public async Task<Result> ActivateUserAsync(string username) {
            var serviceResult = new Result();
            try {
                var applicationUser = await _userManager.FindByNameAsync(username);

                var validationResult = ValidateUserForActivation(applicationUser);
                if (validationResult.Failed)
                    return validationResult;

                applicationUser.IsActive = true;
                var result = await _userManager.UpdateAsync(applicationUser);

                if (result.Succeeded)
                    serviceResult.Successful();
                else
                    serviceResult.Failed().WithMessage($"Unable to activate user.");
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to activate user.");
            }
            return serviceResult;
        }

        public async Task<Result> DeactivateUserAsync(string username) {
            var serviceResult = new Result();
            try {
                var applicationUser = await _userManager.FindByNameAsync(username);

                if (applicationUser == null)
                    return serviceResult.Failed().WithMessage("User not found.");

                if (!applicationUser.IsActive)
                    return serviceResult.Failed().WithMessage("User is not active.");

                applicationUser.IsActive = false;
                var result = await _userManager.UpdateAsync(applicationUser);

                if (result.Succeeded)
                    serviceResult.Successful();
                else
                    serviceResult.Failed().WithMessage($"Unable to deactivate user.");
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to deactivate user.");
            }

            return serviceResult;
        }

        public async Task<Result> AddRolesAsync(RoleAssignmentRequestDTO request) {
            var serviceResult = new Result().Successful();
            try {
                var applicationUser = await _userManager.FindByNameAsync(request.Username);

                if (applicationUser == null)
                    return serviceResult.Failed().WithMessage("User not found.");

                var validationResult = await ValidateRolesForAdditionAsync(request.Roles, applicationUser);
                if (validationResult.Failed)
                    return validationResult;

                var result = await _userManager.AddToRolesAsync(applicationUser, request.Roles.Select(nr => nr.ToUpper()));

                if (result.Succeeded)
                    serviceResult.Successful();
                else
                    serviceResult.Failed().WithMessage($"Unable to add role user.");
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to add role to user.");
            }

            return serviceResult;
        }

        public async Task<Result> RemoveRolesAsync(RoleAssignmentRequestDTO request) {
            var serviceResult = new Result();
            try {
                var applicationUser = await _userManager.FindByNameAsync(request.Username);
                if (applicationUser == null)
                    return serviceResult.Failed().WithMessage("User not found.");

                var validationResult = await ValidateRolesForRemovalAsync(request.Roles, applicationUser);
                if (validationResult.Failed)
                    return validationResult;

                var result = await _userManager.RemoveFromRolesAsync(applicationUser, request.Roles.Select(r => r.ToUpper()));

                if (result.Succeeded)
                    serviceResult.Successful();
                else
                    serviceResult.Failed().WithMessage($"Unable to remove role from user.");
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to remove role from user.");
            }

            return serviceResult;
        }

        public async Task<Result> ChangePasswordAsync(string username, string oldPassword, string newPassword) {
            var serviceResult = new Result();
            try {
                var applicationUser = await _userManager.FindByNameAsync(username);
                if (applicationUser == null) {
                    return serviceResult.Failed().WithMessage("User not found.");
                }

                var changePasswordResult = await _userManager.ChangePasswordAsync(applicationUser, oldPassword, newPassword);
                if (!changePasswordResult.Succeeded) {
                    serviceResult.Failed();
                    foreach (var error in changePasswordResult.Errors) {
                        serviceResult.AddError(error.Description, error.Code);
                    }
                    return serviceResult.WithMessage("Unable to change user password");
                }

                await _signInManager.RefreshSignInAsync(applicationUser);
                serviceResult.Successful();
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to change user password.");
            }

            return serviceResult;
        }

        public async Task<Result> ResetPasswordAsync(string username, string token, string password) {
            var serviceResult = new Result();
            try {
                var applicationUser = await _userManager.FindByNameAsync(username);
                if (applicationUser == null) {
                    return serviceResult.Failed().WithMessage("User not found.");
                }

                var resetPasswordResult = await _userManager.ResetPasswordAsync(applicationUser, token, password);
                if (!resetPasswordResult.Succeeded) {
                    serviceResult.Failed();
                    foreach (var error in resetPasswordResult.Errors) {
                        serviceResult.AddError(error.Description, error.Code);
                    }
                    return serviceResult.WithMessage("Unable to reset user password");
                }

                serviceResult.Successful();
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to reset user password.");
            }

            return serviceResult;
        }

        public async Task<Result<IList<User>>> GetAllUsersAsync(QueryOptions options = null) {
            var serviceResult = new Result<IList<User>>();
            try {
                var query = _userManager.Users.AsNoTracking().Include(u => u.Roles).ThenInclude(ur => ur.Role).AsQueryable();

                if (!string.IsNullOrWhiteSpace(options?.SearchTerm)) {
                    query = query.Where(c => EF.Functions.Like(c.Name, $"%{options.SearchTerm}%"));
                }

                if (!string.IsNullOrWhiteSpace(options?.OrderBy)) {
                    query = query.OrderBy(options.OrderBy);
                }

                if (options?.Skip != null && options?.PageSize != null) {
                    query = query.Skip(options.Skip);
                    query = query.Take(options.PageSize);
                }

                serviceResult.Data = await _mapper.ProjectTo<User>(query).ToListAsync();
                serviceResult.Successful();
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to create user.");
            }
            return serviceResult;
        }

        public async Task<Result<User>> GetUserByIdAsync(string id) {
            var serviceResult = new Result<User>();
            try {
                var userWithRoles = await _userManager.Users.AsNoTracking().Include(u => u.Roles).ThenInclude(ur => ur.Role).FirstOrDefaultAsync(u => u.Id == id);
                if (userWithRoles == null) {
                    serviceResult.Failed().WithError($"Unable to load user with ID '{id}'.");
                    return serviceResult;
                }
                serviceResult.Data = _mapper.Map<User>(userWithRoles);
                serviceResult.Successful();
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to get the user by id.");
            }

            return serviceResult;
        }

        public async Task<Result<User>> GetUserByEmailAsync(string email) {
            var serviceResult = new Result<User>();
            try {
                ApplicationUser user = await _userManager.FindByEmailAsync(email);
                if (user != null) {
                    serviceResult.Data = _mapper.Map<User>(user);
                }

                serviceResult.Successful();
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to get the user by email.");
            }

            return serviceResult;
        }

        public async Task<Result<User>> GetUserByNameAsync(string username) {
            var serviceResult = new Result<User>();
            try {
                ApplicationUser user = await _userManager.FindByNameAsync(username);
                if (user != null) {
                    serviceResult.Data = _mapper.Map<User>(user);
                }

                serviceResult.Successful();
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to get the user by email.");
            }

            return serviceResult;
        }

        public async Task<Result> UpdateRolesAsync(string username, List<string> roles) {
            var serviceResult = new Result();
            try {
                var applicationUser = await _userManager.FindByNameAsync(username);
                if (applicationUser == null) {
                    return serviceResult.Failed().WithMessage("User not found.");
                }

                var existingRoles = await _userManager.GetRolesAsync(applicationUser);
                var identityResult = await _userManager.RemoveFromRolesAsync(applicationUser, existingRoles);

                if (!identityResult.Succeeded) return serviceResult.Failed().WithMessage($"Unable to update user roles.");

                identityResult = await _userManager.AddToRolesAsync(applicationUser, roles.Select(r => r.ToUpper()));

                if (identityResult.Succeeded)
                    serviceResult.Successful();
                else
                    serviceResult.Failed().WithMessage($"Unable to update user roles.");
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to update roles of user.");
            }

            return serviceResult;
        }

        public async Task<Result> UpdateUserDetailsAsync(UpdateUserDetailsDTO request) {
            var serviceResult = new Result();
            try {
                var applicationUser = await _userManager.FindByIdAsync(request.Id);

                if (applicationUser == null)
                    return serviceResult.Failed().WithMessage("User not found.");

                if (!string.IsNullOrWhiteSpace(request.Email) && applicationUser.Email != request.Email)
                    applicationUser.Email = request.Email;

                if (!string.IsNullOrWhiteSpace(request.Name) && applicationUser.Name != request.Name)
                    applicationUser.Name = request.Name;

                if (!string.IsNullOrWhiteSpace(request.PrimaryPhone) && applicationUser.PhoneNumber != request.PrimaryPhone)
                    applicationUser.PhoneNumber = request.PrimaryPhone;

                if (!string.IsNullOrWhiteSpace(request.SecondaryPhone) && applicationUser.SecondaryPhoneNumber != request.SecondaryPhone)
                    applicationUser.SecondaryPhoneNumber = request.SecondaryPhone;

                var result = await _userManager.UpdateAsync(applicationUser);

                if (result.Succeeded)
                    serviceResult.Successful();
                else
                    serviceResult.Failed().WithMessage($"Unable to update user details.");
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to update user details.");
            }

            return serviceResult;
        }

        public async Task<Result<string>> GenerateEmailConfirmationTokenAsync(User user) {
            var serviceResult = new Result<string>();
            try {
                ApplicationUser appUser = _mapper.Map<ApplicationUser>(user);
                string token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

                serviceResult.Successful().WithData(token);
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to generate email confiration token.");
            }
            return serviceResult;
        }

        public async Task<Result> ConfirmUserEmailAsync(string userId, string code) {
            Result serviceResult = new Result();
            try {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) {
                    return serviceResult.Failed().WithError($"Unable to load user with ID '{userId}'.");
                }

                var confirmationResult = await _userManager.ConfirmEmailAsync(user, code);
                if (confirmationResult.Succeeded) {
                    serviceResult.Successful();
                }
                else {
                    serviceResult.Failed();
                }
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to confirm user email.");
            }

            return serviceResult;
        }

        public async Task<Result<string>> GeneratePasswordResetTokenAsync(string userEmail) {
            Result<string> serviceResult = new Result<string>();
            try {
                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user))) {
                    return serviceResult.Failed();
                }

                string code = await _userManager.GeneratePasswordResetTokenAsync(user);
                serviceResult.Successful().WithData(code);
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to generate password reset token.");
            }
            return serviceResult;
        }

        public async Task<Result<bool>> GetTwoFactorEnabledAsync(string username) {
            Result<bool> serviceResult = new Result<bool>();
            try {
                var user = await _userManager.FindByNameAsync(username);
                bool twoFactorIsEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
                serviceResult.Successful().WithData(twoFactorIsEnabled);
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to get two-factor authentication for user.");
            }
            return serviceResult;
        }

        public async Task<Result> SetTwoFactorEnabledAsync(string username, bool enabled) {
            Result serviceResult = new Result();
            try {
                var user = await _userManager.FindByNameAsync(username);
                var twoFactorResult = await _userManager.SetTwoFactorEnabledAsync(user, enabled);
                if (!twoFactorResult.Succeeded) {
                    serviceResult.Failed().WithError($"Cannot enable 2FA for user as it's not currently enabled.");
                }

                serviceResult.Successful();
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to set two-factor authentication for user.");
            }
            return serviceResult;
        }

        public async Task<Result<Dictionary<string, string>>> GetUserPersonalDataAsync(string username) {
            Result<Dictionary<string, string>> serviceResult = new Result<Dictionary<string, string>>();
            try {
                var user = await _userManager.FindByNameAsync(username);
                var personalData = new Dictionary<string, string>();
                var personalDataProps = typeof(ApplicationUser).GetProperties().Where(
                                prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
                foreach (var p in personalDataProps) {
                    personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
                }

                var logins = await _userManager.GetLoginsAsync(user);
                foreach (var l in logins) {
                    personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
                }

                personalData.Add($"Authenticator Key", await _userManager.GetAuthenticatorKeyAsync(user));

                serviceResult.Successful().WithData(personalData);
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to set two-factor authentication for user.");
            }
            return serviceResult;
        }

        private Result<string> ValidateUserForActivation(ApplicationUser applicationUser) {
            var serviceResult = new Result<string>();
            if (applicationUser == null)
                return serviceResult.Failed().WithMessage("User not found.");

            if (applicationUser.IsActive)
                return serviceResult.Failed().WithMessage("User is already active.");
            return serviceResult;
        }

        private async Task<Result> ValidateRolesForAdditionAsync(List<string> roles, ApplicationUser applicationUser) {
            Result validationResult = new Result();
            var rolesAlreadyAssigned = await GetAlreadyAssignedRolesFromUserAsync(roles, applicationUser);
            var invalidRoles = GetInvalidRoles(roles);

            if (rolesAlreadyAssigned.Any())
                return validationResult.Failed().WithErrors(rolesAlreadyAssigned.Select(r => new ResultError($"Role {r} is already assigned.")).ToList());

            if (invalidRoles.Any())
                return validationResult.Failed().WithErrors(invalidRoles.Select(r => new ResultError($"Role {r} is invalid.")).ToList());

            return validationResult.Successful();
        }

        private async Task<Result> ValidateRolesForRemovalAsync(List<string> roles, ApplicationUser applicationUser) {
            Result validationResult = new Result();
            var invalidRoles = GetInvalidRoles(roles);
            var rolesNotAssigned = await GetUnassignedRolesFromUserAsync(roles, applicationUser);

            if (rolesNotAssigned.Any())
                return validationResult.Failed().WithErrors(rolesNotAssigned.Select(r => new ResultError($"Role {r} is not assigned.")).ToList());

            if (invalidRoles.Any())
                return validationResult.Failed().WithErrors(invalidRoles.Select(r => new ResultError($"Role {r} is invalid.")).ToList());

            return validationResult.Successful();
        }

        private List<string> GetInvalidRoles(List<string> rolesToCheck) {
            List<string> invalidRoles = new List<string>();
            foreach (var roleToCheck in rolesToCheck) {
                var isRoleInvalid = !Enum.IsDefined(typeof(RoleEnum), roleToCheck);
                if (isRoleInvalid)
                    invalidRoles.Add(roleToCheck);
            }
            return invalidRoles;
        }

        private async Task<List<string>> GetUnassignedRolesFromUserAsync(List<string> rolesToCheck, ApplicationUser applicationUser) {
            List<string> rolesNotAssigned = new List<string>();
            foreach (var roleToCheck in rolesToCheck) {
                bool isRoleAssigned = await _userManager.IsInRoleAsync(applicationUser, roleToCheck.ToUpper());
                if (isRoleAssigned == false)
                    rolesNotAssigned.Add(roleToCheck);
            }
            return rolesNotAssigned;
        }

        private async Task<List<string>> GetAlreadyAssignedRolesFromUserAsync(List<string> rolesToCheck, ApplicationUser applicationUser) {
            List<string> rolesAssigned = new List<string>();
            foreach (var roleToCheck in rolesToCheck) {
                if (await _userManager.IsInRoleAsync(applicationUser, roleToCheck.ToUpper())) {
                    rolesAssigned.Add(roleToCheck);
                }
            }
            return rolesAssigned;
        }
    }
}
