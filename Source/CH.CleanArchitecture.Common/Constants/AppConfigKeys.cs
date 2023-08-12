namespace CH.CleanArchitecture.Common.Constants
{
    public static class AppConfigKeys
    {
        public static class AUDIT
        {
            public const string PURGE_HISTORYTABLE_INTERVAL_DAYS = "AuditPurgeHistoryTableIntervalDays";
            public const string PURGE_SERVICE_INTERVAL_HOURS = "AuditPurgeServiceIntervalHours";
        }

        public static class EMAIL
        {
            public const string SMTP_SETTINGS = "EmailSmtpSettings";
            public const string FROM_ADDRESS = "EmailFromAddress";
            public const string USE_BCC = "EmailUseBcc";
            public const string BCC_ADDRESS = "EmailBccAddress";
        }

        public static class SECURITY
        {
            public const string GOOGLE_RECAPTCHA_CLIENTKEY = "SecurityGoogleRecaptchaClientKey";
            public const string GOOGLE_RECAPTCHA_SECRETKEY = "SecurityGoogleRecaptchaSecretKey";
        }

        public static class CRYPTO
        {
            public const string JWT_SYMMETRIC_KEY = "CryptoJWTSymmetricKey";
            public const string JWT_ISSUER = "CryptoJWTIssuer";
            public const string JWT_AUTHORITY = "CryptoJWTAuthority";
        }

        public static class EVENTSTORE
        {
            public const string SNAPSHOT_FREQUENCY = "EventStoreSnapshotFrequency";
        }

        public static class NOTIFICATIONS
        {
            public const string PURGE_HISTORYTABLE_INTERVAL = "NotificationsPurgeHistoryTableInterval";
            public const string PURGE_SERVICE_INTERVAL_HOURS = "NotificationsPurgeServiceIntervalHours";
        }
    }
}
