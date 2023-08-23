using System;
using System.Collections.Generic;
using System.Linq;
using CH.CleanArchitecture.Core.Application;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace CH.CleanArchitecture.Infrastructure.Services
{
    internal class PasswordGeneratorIdentityService : IPasswordGeneratorService
    {
        private readonly IOptions<IdentityOptions> _identityOptions;

        public PasswordGeneratorIdentityService(IOptions<IdentityOptions> identityOptions) {
            _identityOptions = identityOptions;
        }

        /// <summary>
        /// Generates a password according to Identity password options configured
        /// </summary>
        /// <returns></returns>
        public string GenerateRandomPassword() {
            var passwordOpt = _identityOptions.Value.Password;
            bool requiredDigit = passwordOpt.RequireDigit;
            int requiredUniqueChars = passwordOpt.RequiredUniqueChars;
            int requiredLength = passwordOpt.RequiredLength;
            bool requireLowercase = passwordOpt.RequireLowercase;
            bool requireNonAlpha = passwordOpt.RequireNonAlphanumeric;
            bool requireUppercase = passwordOpt.RequireUppercase;
            return GeneratePassword(requiredLength, requiredUniqueChars, requiredDigit, requireLowercase, requireNonAlpha, requireUppercase);
        }

        /// <summary>
        /// Generates a Random Password
        /// respecting the given strength requirements.
        /// </summary>
        /// <param name="opts">A valid PasswordOptions object
        /// containing the password strength requirements.</param>
        /// <returns>A random password</returns>
        private string GeneratePassword(int requiredLength = 6, int requiredUniqueChars = 4, bool requireDigit = true, bool requireLowercase = true, bool requireNonAlphanumeric = true, bool requireUppercase = true) {
            string[] randomChars = new[] {
            "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
            "abcdefghijkmnopqrstuvwxyz",    // lowercase
            "0123456789",                   // digits
            @"-._@+\"                       // non-alphanumeric
            };

            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (requireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (requireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (requireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (requireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < requiredLength
                || chars.Distinct().Count() < requiredUniqueChars; i++) {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }
    }
}
