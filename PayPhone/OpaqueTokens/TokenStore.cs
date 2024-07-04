namespace PayPhone.OpaqueTokens
{
    public static class TokenStore
    {
        private static readonly Dictionary<string, TokenInfo> Tokens = new();

        public static string GenerateToken(int userId)
        {
            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 22);
            var tokenInfo = new TokenInfo
            {
                UserId = userId,
                Expiry = DateTime.UtcNow.AddMinutes(30) // Token valid for 30 minutes
            };

            Tokens[token] = tokenInfo;
            return token;
        }

        public static TokenInfo? GetTokenInfo(string token)
        {
            if (Tokens.TryGetValue(token, out var tokenInfo) && tokenInfo.Expiry > DateTime.UtcNow)
            {
                return tokenInfo;
            }
            return null;
        }

        public static void RemoveToken(string token)
        {
            Tokens.Remove(token);
        }
    }
}
