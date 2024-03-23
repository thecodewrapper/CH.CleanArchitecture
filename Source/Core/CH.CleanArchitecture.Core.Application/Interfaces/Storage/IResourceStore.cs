using System.IO;
using System.Threading.Tasks;

namespace CH.CleanArchitecture.Core.Application
{
    /// <summary>
    /// Abstraction for a resource/file store
    /// </summary>
    public interface IResourceStore
    {
        Task SaveResourceAsync(Stream stream, string path, string resourceId);
        Task<string> SaveResourceAsync(Stream stream, string path);
        Task<Stream> DownloadResourceAsync(string path, string resourceId);
        Task<bool> DeleteResourceAsync(string path, string resourceId);
        string GetResourceURI(string path, string resourceId);
    }
}
