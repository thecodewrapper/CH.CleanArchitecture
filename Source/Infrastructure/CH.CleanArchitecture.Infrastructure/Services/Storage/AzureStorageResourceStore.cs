using Azure.Identity;
using Azure.Storage.Blobs;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Common.Constants;
using CH.CleanArchitecture.Core.Application;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CH.CleanArchitecture.Infrastructure.Services
{
    /// <summary>
    /// Implementation of <see cref="IResourceStore"/> using Azure Storage as the underlying store.
    /// This implementation retrieves relevant options from application configurations. You may change this to use <see cref="StorageOptions"/> along with any configuration required.
    /// </summary>
    internal class AzureStorageResourceStore : IResourceStore
    {
        #region Private Fields

        private readonly IApplicationConfigurationService _appConfigService;
        private readonly string _baseUri;
        private readonly ILogger<AzureStorageResourceStore> _logger;

        #endregion Private Fields

        #region Public Constructors

        public AzureStorageResourceStore(ILogger<AzureStorageResourceStore> logger, IApplicationConfigurationService appConfigService) {
            _logger = logger;
            _appConfigService = appConfigService;
            _baseUri = _appConfigService.GetValue(AppConfigKeys.AZURE.BLOB_STORAGE_BASE_URI).Unwrap();
        }

        #endregion Public Constructors

        #region Public Methods

        public string GetResourceURI(string containerName, string resourceId) {
            return $"{_baseUri}{containerName}/{resourceId}";
        }

        public async Task<bool> DeleteResourceAsync(string containerName, string resourceId) {
            try {
                var blob = GetBlobReference(containerName, resourceId);
                _logger.LogDebug($"Attempting to delete resource {resourceId} from Azure Storage Blob (Container: {containerName}).");
                return await blob.DeleteIfExistsAsync();
            }
            catch (Exception ex) {
                _logger.LogError(ex, $"Failed to delete resource {resourceId} from Azure Storage Blob (Container: {containerName}).");
                throw;
            }
        }

        public async Task SaveResourceAsync(Stream stream, string containerName, string resourceId) {
            try {
                var blob = GetBlobReference(containerName, resourceId);
                await blob.UploadAsync(stream);
                _logger.LogDebug($"Saved resource {resourceId} successfully to Azure Storage Blob (Container: {containerName}, Stream length: {stream.Length})");
            }
            catch (Exception ex) {
                _logger.LogError(ex, $"Failed to save resource {resourceId} to Azure Storage Blob (Container: {containerName}, Stream length: {stream.Length})");
                throw;
            }
        }

        public async Task<string> SaveResourceAsync(Stream imageStream, string containerName) {
            string resourceId = Guid.NewGuid().ToString(); //generating a random resource id
            await SaveResourceAsync(imageStream, containerName, resourceId);
            return resourceId;
        }

        public async Task<Stream> DownloadResourceAsync(string containerName, string resourceId) {
            try {
                _logger.LogInformation($"Downloading resource '{containerName}'/{resourceId}");

                var blob = GetBlobReference(containerName, resourceId);
                MemoryStream memoryStream = new MemoryStream();
                await blob.DownloadToAsync(memoryStream);

                return memoryStream;
            }
            catch (Exception ex) {
                _logger.LogError(ex, $"Failed to download resource {resourceId} from Azure Storage Blob (Container: {containerName}");
                throw;
            }
        }

        #endregion Public Methods

        #region Private Methods

        private BlobClient GetBlobReference(string containerName, string resourceId) {
            var blobServiceClient = GetBlobServiceClient();
            var container = blobServiceClient.GetBlobContainerClient(containerName);
            var blob = container.GetBlobClient(resourceId);
            return blob;
        }

        private BlobServiceClient GetBlobServiceClient() {
            bool usePasswordlessAuthentication = _appConfigService.GetValueBool(AppConfigKeys.AZURE.STORAGE_USE_PASSWORDLESS_AUTHENTICATION).Unwrap();

            if (usePasswordlessAuthentication) {
                string azureStorageAccountName = _appConfigService.GetValue(AppConfigKeys.AZURE.STORAGE_ACCOUNT_NAME).Unwrap();
                // https://learn.microsoft.com/en-us/dotnet/api/overview/azure/identity-readme?view=azure-dotnet#defaultazurecredential
                return new BlobServiceClient(new Uri(azureStorageAccountName), new DefaultAzureCredential());
            }
            else {
                string connString = _appConfigService.GetValue(AppConfigKeys.AZURE.STORAGE_CONNECTION_STRING).Unwrap();
                return new BlobServiceClient(connString);
            }
        }

        #endregion Private Methods
    }
}
