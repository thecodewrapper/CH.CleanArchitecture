using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Common.Constants;
using CH.CleanArchitecture.Core.Application;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace CH.CleanArchitecture.Infrastructure.Services
{
    internal class JWTService : IJWTService
    {
        private readonly ILogger<JWTService> _logger;
        private readonly IApplicationConfigurationService _applicationConfigurationService;
        private readonly string _symmetricKey;
        private readonly string _issuer;
        private readonly string _authority;
        private const string PAYLOAD_CLAIMS_TYPE = "payload";

        public JWTService(ILogger<JWTService> logger, IApplicationConfigurationService applicationConfigurationService) {
            _logger = logger;
            _applicationConfigurationService = applicationConfigurationService;
            _symmetricKey = _applicationConfigurationService.GetValue(AppConfigKeys.CRYPTO.JWT_SYMMETRIC_KEY).Unwrap();
            _issuer = _applicationConfigurationService.GetValue(AppConfigKeys.CRYPTO.JWT_ISSUER).Unwrap();
            _authority = _applicationConfigurationService.GetValue(AppConfigKeys.CRYPTO.JWT_AUTHORITY).Unwrap();
        }

        /// <summary>
        /// Creates a JWT Token
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="daysValid"></param>
        /// <returns></returns>
        public string CreateJWT(object payload, int daysValid) {
            var tokenHandler = new JwtSecurityTokenHandler();
            var claimsIdentity = CreateClaimsIdentity(payload);

            // Create JWToken
            var token = tokenHandler.CreateJwtSecurityToken(issuer: _issuer,
                audience: _authority,
                subject: claimsIdentity,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(daysValid),
                signingCredentials:
                new SigningCredentials(
                    new SymmetricSecurityKey(
                        Encoding.Default.GetBytes(_symmetricKey)),
                        SecurityAlgorithms.HmacSha256Signature));

            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Validates and reads a JWT token created previously by this service and returns the underlying payload
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jwt"></param>
        /// <returns></returns>
        public T ReadJWT<T>(string jwt) {
            if (ValidateJWT(jwt)) {
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(jwt);
                return JsonConvert.DeserializeObject<T>(token.Claims.First(c => c.Type == PAYLOAD_CLAIMS_TYPE).Value);
            }
            return default;
        }

        private bool ValidateJWT(string jwt) {
            try {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = GetValidationParameters();

                tokenHandler.ValidateToken(jwt, validationParameters, out SecurityToken securityToken); //throws if not valid
                return true;
            }
            catch {
                _logger.LogError($"Invalid JWT token");
                return false;
            }
        }

        private TokenValidationParameters GetValidationParameters() {
            return new TokenValidationParameters()
            {
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidAudience = _authority,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_symmetricKey)) // The same key as the one that generate the token
            };
        }

        private ClaimsIdentity CreateClaimsIdentity(object payload) {
            ClaimsIdentity claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(PAYLOAD_CLAIMS_TYPE, JsonConvert.SerializeObject(payload)));

            return claimsIdentity;
        }
    }
}
