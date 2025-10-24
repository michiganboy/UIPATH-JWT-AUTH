using System;
using System.Text.Json;

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
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseBody, options);
                
                if (tokenResponse == null)
                {
                    throw new Exception("Failed to parse token response");
                }
                
                return tokenResponse;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to parse token response: {ex.Message}", ex);
            }
        }
    }
}
