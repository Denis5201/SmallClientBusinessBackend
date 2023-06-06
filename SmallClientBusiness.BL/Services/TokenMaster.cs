using Microsoft.IdentityModel.Tokens;
using SmallClientBusiness.Common.System;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SmallClientBusiness.BL.Services
{
    internal static class TokenMaster
    {
        public static string CreateAccessToken(List<Claim> claims, JwtConfigs configs)
        {
            var nowTime = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                issuer: configs.Issuer,
                audience: configs.Audience,
                notBefore: nowTime,
                claims: claims,
                expires: nowTime.AddMinutes(configs.JwtLifeTimeMin),
                signingCredentials: new SigningCredentials(configs.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public static string CreateRefreshToken()
        {
            var number = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(number);

            return Convert.ToBase64String(number);
        }

        public static string? GetIdByOldToken(string oldJwtToken, JwtConfigs configs)
        {
            var tokenVailidParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = configs.Issuer,
                ValidateAudience = true,
                ValidAudience = configs.Audience,
                ValidateLifetime = false,
                IssuerSigningKey = configs.GetSymmetricSecurityKey(),
                ValidateIssuerSigningKey = true
            };

            var principal = new JwtSecurityTokenHandler()
                .ValidateToken(oldJwtToken, tokenVailidParams, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken token
                || !token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
