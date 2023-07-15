namespace CH.CleanArchitecture.Core.Application
{
    public interface IUrlTokenService
    {
        /// <summary>
        /// Creates a safe url token containing the given payload
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        string CreateSafeUrlToken(object payload);

        /// <summary>
        /// Reads a safe url token and returns the containing payload
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token"></param>
        /// <returns></returns>
        T ReadSafeUrlToken<T>(string token);
    }
}
