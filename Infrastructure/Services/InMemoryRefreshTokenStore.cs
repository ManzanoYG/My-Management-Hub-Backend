using System.Collections.Concurrent;

namespace Infrastructure.Services
{
    public class InMemoryRefreshTokenStore : IRefreshTokenStore
    {
        private sealed class RefreshTokenRecord
        {
            public string TokenHash { get; set; } = string.Empty;
            public DateTime ExpiresAtUtc { get; set; }
        }

        private readonly ConcurrentDictionary<string, RefreshTokenRecord> _tokens = new();

        public void Save(string username, string refreshTokenHash, DateTime expiresAtUtc)
        {
            _tokens[username] = new RefreshTokenRecord
            {
                TokenHash = refreshTokenHash,
                ExpiresAtUtc = expiresAtUtc
            };
        }

        public bool Validate(string username, string refreshTokenHash)
        {
            if (!_tokens.TryGetValue(username, out var tokenRecord))
            {
                return false;
            }

            if (tokenRecord.ExpiresAtUtc <= DateTime.UtcNow)
            {
                _tokens.TryRemove(username, out _);
                return false;
            }

            return string.Equals(tokenRecord.TokenHash, refreshTokenHash, StringComparison.Ordinal);
        }

        public void Rotate(string username, string oldRefreshTokenHash, string newRefreshTokenHash, DateTime newExpiresAtUtc)
        {
            if (!Validate(username, oldRefreshTokenHash))
            {
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }

            Save(username, newRefreshTokenHash, newExpiresAtUtc);
        }

        public void Revoke(string username)
        {
            _tokens.TryRemove(username, out _);
        }
    }
}
