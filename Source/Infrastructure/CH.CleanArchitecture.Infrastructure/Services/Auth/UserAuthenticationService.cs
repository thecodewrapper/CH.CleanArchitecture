using System;
using System.Threading.Tasks;
using AutoMapper;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Core.Application.DTOs;
using CH.CleanArchitecture.Core.Domain.Entities.UserAggregate;
using CH.CleanArchitecture.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CH.CleanArchitecture.Infrastructure.Services
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<UserAuthenticationService> _logger;

        public UserAuthenticationService(ILogger<UserAuthenticationService> logger, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IMapper mapper) {
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<LoginResponseDTO>> Login(LoginRequestDTO loginRequest) {
            var serviceResult = new Result<LoginResponseDTO>();
            try {
                var applicationUser = await _userManager.FindByNameAsync(loginRequest?.Username);
                var validationResult = ValidateUserForLogin(applicationUser);
                if (validationResult.IsFailed)
                    return serviceResult.Fail();

                return await SignInUser(applicationUser, loginRequest);
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to sign in user.");
            }
            return serviceResult;
        }

        public async Task<Result<LoginResponseDTO>> LoginWith2fa(Login2FARequest login2FARequest) {
            var serviceResult = new Result<LoginResponseDTO>();
            try {
                var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                var validationResult = ValidateUserForLogin(user);
                if (validationResult.IsFailed)
                    return serviceResult.Fail();

                return await SignInUser2fa(user, login2FARequest);
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to sign in user with 2FA.");
            }
            return serviceResult;
        }

        public async Task<Result<LoginResponseDTO>> LoginWithRecoveryCode(Login2FARequest login2FARequest) {
            var serviceResult = new Result<LoginResponseDTO>();
            try {
                var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                var validationResult = ValidateUserForLogin(user);
                if (validationResult.IsFailed)
                    return serviceResult.Fail();

                return await SignInUserRecoveryCode(user, login2FARequest);
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to sign in user with recovery code.");
            }
            return serviceResult;
        }

        public async Task<Result> Logout() {
            var serviceResult = new Result<string>();
            try {
                await _signInManager.SignOutAsync();
                serviceResult.Succeed().WithMessage("Successfully logged out.");
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to sign out user.");
            }
            return serviceResult;
        }

        public async Task<Result<User>> GetTwoFactorAuthenticationUserAsync() {
            var serviceResult = new Result<User>();
            try {
                var appUser = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                if (appUser == null) {
                    return serviceResult.Fail().WithError("Unable to retrieve two-factor authentication user");
                }
                return serviceResult.Succeed().WithData(_mapper.Map<User>(appUser));
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to retrieve two factor authentication user.");
            }
            return serviceResult;
        }

        private Result<string> ValidateUserForLogin(ApplicationUser applicationUser) {
            var serviceResult = new Result<string>();
            if (applicationUser == null)
                return serviceResult.Fail().WithMessage("User not found.");

            if (applicationUser.IsActive == false)
                return serviceResult.Fail().WithMessage("User is not active.");
            return serviceResult;
        }

        private async Task<Result<LoginResponseDTO>> SignInUser(ApplicationUser applicationUser, LoginRequestDTO loginRequest) {
            var serviceResult = new Result<LoginResponseDTO>();
            var loginResult = await _signInManager.PasswordSignInAsync(applicationUser, loginRequest.Password, loginRequest.RememberMe, false);

            serviceResult.Succeed().WithData(ConstructLoginResponseDTO(applicationUser, loginResult));
            return serviceResult;
        }

        private async Task<Result<LoginResponseDTO>> SignInUser2fa(ApplicationUser applicationUser, Login2FARequest login2faRequest) {
            var serviceResult = new Result<LoginResponseDTO>();
            var loginResult = await _signInManager.TwoFactorAuthenticatorSignInAsync(login2faRequest.Code, login2faRequest.IsPersisted, login2faRequest.RememberClient);

            serviceResult.Succeed().WithData(ConstructLoginResponseDTO(applicationUser, loginResult));
            return serviceResult;
        }

        private async Task<Result<LoginResponseDTO>> SignInUserRecoveryCode(ApplicationUser applicationUser, Login2FARequest login2faRequest) {
            var serviceResult = new Result<LoginResponseDTO>();
            var loginResult = await _signInManager.TwoFactorRecoveryCodeSignInAsync(login2faRequest.Code);

            serviceResult.Succeed().WithData(ConstructLoginResponseDTO(applicationUser, loginResult));
            return serviceResult;
        }

        private LoginResponseDTO ConstructLoginResponseDTO(ApplicationUser user, SignInResult signInResult) {
            var response = new LoginResponseDTO()
            {
                User = user.UserName,
                Success = signInResult.Succeeded,
                IsLockedOut = signInResult.IsLockedOut,
                Requires2FA = signInResult.RequiresTwoFactor,
                IsNotAllowed = signInResult.IsNotAllowed
            };

            _logger.LogDebug($"Returning {nameof(LoginResponseDTO)}. {response}");
            return response;
        }
    }
}
