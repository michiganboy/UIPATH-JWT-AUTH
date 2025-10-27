using System;
using Newtonsoft.Json;

namespace SalesforceJWT
{
    public class TokenResponse
    {
        public string access_token { get; set; }
        public string instance_url { get; set; }
        public string id { get; set; }
        public string token_type { get; set; }
        public string issued_at { get; set; }
        public string signature { get; set; }
        public int expires_in { get; set; }
    }
    
    public class TokenParser
    {
        public TokenResponse ParseTokenResponse(string responseBody)
        {
            try
            {
                if (string.IsNullOrEmpty(responseBody))
                {
                    throw new ArgumentException("Response body cannot be null or empty", nameof(responseBody));
                }
                
                // Validate JSON format
                if (!responseBody.Trim().StartsWith("{") || !responseBody.Trim().EndsWith("}"))
                {
                    throw new ArgumentException("Response body must be valid JSON", nameof(responseBody));
                }
                
                var options = new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseBody, options);
                
                if (tokenResponse == null)
                {
                    throw new Exception("Failed to parse token response - deserialization returned null");
                }
                
                // Validate required fields
                if (string.IsNullOrEmpty(tokenResponse.access_token))
                {
                    throw new Exception("Token response is missing required 'access_token' field");
                }
                
                if (string.IsNullOrEmpty(tokenResponse.instance_url))
                {
                    throw new Exception("Token response is missing required 'instance_url' field");
                }
                
                return tokenResponse;
            }
            catch (JsonException jsonEx)
            {
                throw new Exception($"Failed to parse token response - invalid JSON: {jsonEx.Message}", jsonEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to parse token response: {ex.Message}", ex);
            }
        }
    }
}