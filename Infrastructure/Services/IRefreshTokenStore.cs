namespace Infrastructure.Services
{
    public interface IRefreshTokenStore
    {
        void Save(string username, string refreshTokenHash, DateTime expiresAtUtc);
        bool Validate(string username, string refreshTokenHash);
        void Rotate(string username, string oldRefreshTokenHash, string newRefreshTokenHash, DateTime newExpiresAtUtc);
        void Revoke(string username);
    }
}
