namespace CH.CleanArchitecture.Common
{
    public interface IResultError
    {
        string Error { get; }
        string Code { get; }
    }
}