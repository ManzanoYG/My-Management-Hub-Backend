namespace Infrastructure.Services
{
    public static class AuditActions
    {
        // Authentication
        public const string UserLogin = "USER_LOGIN";
        public const string UserLoginFailed = "USER_LOGIN_FAILED";
        public const string UserLogout = "USER_LOGOUT";
        public const string TokenRefreshed = "TOKEN_REFRESHED";
        public const string TokenRefreshFailed = "TOKEN_REFRESH_FAILED";

        // User management
        public const string UserCreated = "USER_CREATED";
        public const string UserUpdated = "USER_UPDATED";
        public const string UserDeleted = "USER_DELETED";
        public const string PasswordChanged = "PASSWORD_CHANGED";
        public const string AccountLocked = "ACCOUNT_LOCKED";
        public const string AccountUnlocked = "ACCOUNT_UNLOCKED";

        // Admin / permissions
        public const string AdminAction = "ADMIN_ACTION";
        public const string PermissionGranted = "PERMISSION_GRANTED";
        public const string PermissionRevoked = "PERMISSION_REVOKED";
        public const string RoleChanged = "ROLE_CHANGED";
    }
}
