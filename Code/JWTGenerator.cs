using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SalesforceJWT
{
    public class JWTGenerator
    {
        public string GenerateJWT(string clientId, string username, string loginUrl, string privateKey)
        {
            try
            {
                // Parse the private key
                var rsa = RSA.Create();
                rsa.ImportFromPem(privateKey);
                
                // Create signing credentials
                var key = new RsaSecurityKey(rsa);
                var credentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);
                
                // Create JWT token
                var token = new JwtSecurityToken(
                    issuer: clientId,
                    audience: loginUrl,
                    claims: new[]
                    {
                        new System.Security.Claims.Claim("sub", username),
                        new System.Security.Claims.Claim("iss", clientId),
                        new System.Security.Claims.Claim("aud", loginUrl),
                        new System.Security.Claims.Claim("exp", DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeSeconds().ToString(), "http://www.w3.org/2001/XMLSchema#integer")
                    },
                    expires: DateTime.UtcNow.AddMinutes(5),
                    signingCredentials: credentials
                );
                
                // Serialize token
                var tokenHandler = new JwtSecurityTokenHandler();
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to generate JWT: {ex.Message}", ex);
            }
        }
    }
}
