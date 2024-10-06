using Microsoft.IdentityModel.Tokens;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Implements
{
    public class JwtTokenHelper : IJwtTokenHelper
    {
        private readonly string _secretKey;

        public JwtTokenHelper(string secretKey)
        {
            _secretKey = secretKey;
        }

        public ClaimsPrincipal DecodeToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Convert.FromBase64String(_secretKey);

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,  // Set to true if you want to validate the issuer
                    ValidateAudience = false, // Set to true if you want to validate the audience
                    ValidateLifetime = true, // Validate token expiration
                    ClockSkew = TimeSpan.Zero // Optional: To avoid time discrepancies
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                if (validatedToken is JwtSecurityToken jwtToken)
                {
                    if (jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return principal; // Return the principal (decoded claims)
                    }
                }

                return null;
            }
            catch (SecurityTokenException)
            {
                // Handle invalid token
                return null;
            }
            catch (Exception)
            {
                // Handle other errors
                return null;
            }
        }

        public string GetClaimValue(string token, string claimType)
        {
            var principal = DecodeToken(token);
            return principal?.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
        }
    }
}
