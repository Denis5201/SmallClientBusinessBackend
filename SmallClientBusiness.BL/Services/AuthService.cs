using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SmallClientBusiness.Common.Dto;
using SmallClientBusiness.Common.Exceptions;
using SmallClientBusiness.Common.Interfaces;
using SmallClientBusiness.Common.System;
using SmallClientBusiness.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmallClientBusiness.BL.Services
{
    internal class AuthService : IAuthService
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly JwtConfigs _jwtConfigs;

        public AuthService(UserManager<UserEntity> userManager, IOptions<JwtConfigs> options)
        {
            _userManager = userManager;
            _jwtConfigs = options.Value;
        }

        public async Task<TokenPair> CreateWorker(CreateWorkerUser createWorker)
        {
            if (createWorker.BirthDate >= DateOnly.FromDateTime(DateTime.UtcNow))
            {
                throw new IncorrectDataException("Дата рождения должна быть меньше текущей");
            }

            var existingUser = await _userManager.FindByEmailAsync(createWorker.Email);
            if (existingUser != null)
            {
                throw new IncorrectDataException("Пользователь с данным E-mail уже существует");
            }

            var user = new UserEntity
            {
                Email = createWorker.Email,
                UserName = createWorker.FullName,
                BirthDate = createWorker.BirthDate.ToDateTime(TimeOnly.MinValue).ToUniversalTime(),
                PhoneNumber = createWorker.PhoneNumber,
                WorkerEntity = new WorkerEntity()
            };

            var result = await _userManager.CreateAsync(user, createWorker.Password);
            if (!result.Succeeded)
            {
                throw new IncorrectDataException(result.Errors.First().Description);
            }

            await _userManager.AddToRoleAsync(user, AppRoles.Worker);

            var tokenPair = await GetTokenPair(user);

            return tokenPair;
        }

        public async Task<TokenPair> Login(LoginCredentials credentials)
        {
            var user = await _userManager.FindByEmailAsync(credentials.Email);

            if (user == null)
            {
                throw new IncorrectDataException("Неверный логин или пароль");
            }
            if (!await _userManager.CheckPasswordAsync(user, credentials.Password))
            {
                throw new IncorrectDataException("Неверный логин или пароль");
            }

            var tokenPair = await GetTokenPair(user);

            return tokenPair;
        }

        public async Task<TokenPair> Refresh(TokenPair oldTokens)
        {
            var userId = TokenMaster.GetIdByOldToken(oldTokens.AccessToken, _jwtConfigs);
            if (userId == null)
            {
                throw new IncorrectDataException("Некорректный токен");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.RefreshToken != oldTokens.RefreshToken || user.RefreshTokenExpires <= DateTime.UtcNow)
            {
                throw new IncorrectDataException("Неверный токен");
            }

            var tokenPair = await GetTokenPair(user);

            return tokenPair;
        }

        public async Task Logout(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new IncorrectDataException("Неверный токен");
            }

            user.RefreshToken = null;
            user.RefreshTokenExpires = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
        }

        private async Task<TokenPair> GetTokenPair(UserEntity user)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) };
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var accessToken = TokenMaster.CreateAccessToken(claims, _jwtConfigs);
            var refreshToken = TokenMaster.CreateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpires = DateTime.UtcNow.AddDays(_jwtConfigs.RefreshLifeTimeDay);
            await _userManager.UpdateAsync(user);

            return new TokenPair
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}
