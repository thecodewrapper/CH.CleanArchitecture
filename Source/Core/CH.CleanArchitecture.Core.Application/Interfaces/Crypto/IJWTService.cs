namespace CH.CleanArchitecture.Core.Application
{
    public interface IJWTService
    {
        /// <summary>
        /// Creates a JWT Token
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="daysValid"></param>
        /// <returns></returns>
        string CreateJWT(object payload, int daysValid);

        /// <summary>
        /// Validates and reads a JWT token created previously by this service and returns the underlying payload
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jwt"></param>
        /// <returns></returns>
        T ReadJWT<T>(string jwt);
    }
}
