using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Infrastructure.Options;
using Microsoft.Extensions.Options;
using NetBox.Extensions;
using Storage.Net;
using Storage.Net.Blobs;

namespace CH.CleanArchitecture.Infrastructure.Services
{
    internal class FileStorageService : IFileStorageService
    {
        private readonly IBlobStorage _storage;
        private readonly string _basePath;

        public FileStorageService(IOptions<FileStorageOptions> options) {
            _basePath = options.Value.BasePath;
            _storage = StorageFactory.Blobs.DirectoryFiles(_basePath);
        }

        public async Task DeleteAsync(string filePath, CancellationToken cancellationToken = default) {
            await _storage.DeleteAsync(filePath.AsEnumerable(), cancellationToken);
        }
        public async Task<bool> ExistsAsync(string filePath, CancellationToken cancellationToken = default) {
            var result = await _storage.ExistsAsync(filePath.AsEnumerable(), cancellationToken);
            return result.First();
        }
        public async Task<Stream> OpenReadAsync(string filePath, CancellationToken cancellationToken = default) {
            return new FileStream(filePath, FileMode.Open);
        }
        public async Task WriteAsync(string filePath, Stream dataStream, bool overwrite = false, CancellationToken cancellationToken = default) {
            await using FileStream fs = new(filePath, FileMode.Create);
            await dataStream.CopyToAsync(fs, cancellationToken);
            dataStream.Close();
        }
    }
}
