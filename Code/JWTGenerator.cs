using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.IO;

namespace SalesforceJWT
{
    public class JWTGenerator
    {
        public string GenerateJWT(string clientId, string username, string loginUrl, string privateKey)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrEmpty(clientId))
                    throw new ArgumentException("ClientId cannot be null or empty", nameof(clientId));
                
                if (string.IsNullOrEmpty(username))
                    throw new ArgumentException("Username cannot be null or empty", nameof(username));
                
                if (string.IsNullOrEmpty(loginUrl))
                    throw new ArgumentException("LoginUrl cannot be null or empty", nameof(loginUrl));
                
                if (string.IsNullOrEmpty(privateKey))
                    throw new ArgumentException("PrivateKey cannot be null or empty", nameof(privateKey));
                
                // Validate URL format
                if (!Uri.TryCreate(loginUrl, UriKind.Absolute, out Uri uri) || (uri.Scheme != "https" && uri.Scheme != "http"))
                    throw new ArgumentException("LoginUrl must be a valid HTTP or HTTPS URL", nameof(loginUrl));
                
                // Validate private key format
                if (!privateKey.Contains("-----BEGIN") || !privateKey.Contains("-----END"))
                    throw new ArgumentException("PrivateKey must be in PEM format", nameof(privateKey));
                
                // Parse the private key using X509Certificate2
                var rsa = ImportPrivateKeyFromPem(privateKey);
                
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
                        new System.Security.Claims.Claim("exp", ((DateTimeOffset.UtcNow.AddMinutes(5) - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds).ToString(), "http://www.w3.org/2001/XMLSchema#integer")
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
        
        private static RSA ImportPrivateKeyFromPem(string pemContent)
        {
            try
            {
                // Remove PEM headers and footers
                var base64String = pemContent
                    .Replace("-----BEGIN PRIVATE KEY-----", "")
                    .Replace("-----END PRIVATE KEY-----", "")
                    .Replace("-----BEGIN RSA PRIVATE KEY-----", "")
                    .Replace("-----END RSA PRIVATE KEY-----", "")
                    .Replace("\r", "")
                    .Replace("\n", "")
                    .Replace(" ", "");
                
                // Decode base64 to get DER encoded key
                var derBytes = Convert.FromBase64String(base64String);
                
                // Parse ASN.1 structure to extract RSA parameters
                var rsaParams = ParseRSAPrivateKey(derBytes);
                
                // Create RSA with extracted parameters
                var rsa = new RSACryptoServiceProvider();
                rsa.ImportParameters(rsaParams);
                
                return rsa;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to import private key from PEM: {ex.Message}", ex);
            }
        }
        
        private static RSAParameters ParseRSAPrivateKey(byte[] derBytes)
        {
            // This is a simplified ASN.1 parser for RSA private keys
            // For production use, consider using a proper ASN.1 library
            
            int offset = 0;
            
            // Skip ASN.1 sequence header
            if (derBytes[offset] != 0x30) // SEQUENCE
                throw new Exception("Invalid ASN.1 structure");
            
            offset += 2; // Skip length bytes
            
            // Skip version (should be 0)
            if (derBytes[offset] == 0x02) // INTEGER
            {
                offset += 2 + derBytes[offset + 1];
            }
            
            // Extract modulus (n)
            var modulus = ExtractASN1Integer(derBytes, ref offset);
            
            // Extract public exponent (e)
            var exponent = ExtractASN1Integer(derBytes, ref offset);
            
            // Extract private exponent (d)
            var d = ExtractASN1Integer(derBytes, ref offset);
            
            // Extract prime1 (p)
            var p = ExtractASN1Integer(derBytes, ref offset);
            
            // Extract prime2 (q)
            var q = ExtractASN1Integer(derBytes, ref offset);
            
            // Extract exponent1 (dp)
            var dp = ExtractASN1Integer(derBytes, ref offset);
            
            // Extract exponent2 (dq)
            var dq = ExtractASN1Integer(derBytes, ref offset);
            
            // Extract coefficient (qinv)
            var qinv = ExtractASN1Integer(derBytes, ref offset);
            
            return new RSAParameters
            {
                Modulus = modulus,
                Exponent = exponent,
                D = d,
                P = p,
                Q = q,
                DP = dp,
                DQ = dq,
                InverseQ = qinv
            };
        }
        
        private static byte[] ExtractASN1Integer(byte[] data, ref int offset)
        {
            if (data[offset] != 0x02) // INTEGER
                throw new Exception("Expected ASN.1 INTEGER");
            
            int length = data[offset + 1];
            offset += 2;
            
            var result = new byte[length];
            Array.Copy(data, offset, result, 0, length);
            offset += length;
            
            return result;
        }
    }
}