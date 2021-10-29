using System;
using System.IO;

namespace CH.CleanArchitecture.Common.Extensions
{
    public static class StreamExtensions
    {
        public static string ToBase64String(this Stream stream) {
            byte[] bytes;
            using (var memoryStream = new MemoryStream()) {
                stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
                stream.Close();
            }

            return Convert.ToBase64String(bytes);
        }
    }
}
