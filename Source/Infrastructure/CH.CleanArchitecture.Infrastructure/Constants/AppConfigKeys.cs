namespace CH.CleanArchitecture.Infrastructure.Constants
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
            public const string SENDGRID_API_KEY = "EmailSendGridApiKey";
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

        public static class AZURE
        {
            public const string STORAGE_CONNECTION_STRING = "AzureStorageConnectionString";
            public const string BLOB_STORAGE_BASE_URI = "AzureBlobStorageBaseURI";
            public const string STORAGE_USE_PASSWORDLESS_AUTHENTICATION = "AzureStorageUsePasswordlessAuthentication";
            public const string STORAGE_ACCOUNT_NAME = "AzureStorageAccountName";
        }

        public static class AWS
        {
            public const string S3_REGION = "AWSS3Region";
            public const string S3_BUCKET_NAME = "AWSS3BucketName";
            public const string S3_ENDPOINT_FORMAT = "AWSS3EndpointFormat";
            public const string AWS_ACCESS_KEY_ID = "AWSAccessKeyId";
            public const string AWS_SECRET_ACCESS_KEY = "AWSSecretAccessKey";
        }
    }
}
