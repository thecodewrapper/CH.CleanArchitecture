namespace CH.CleanArchitecture.Core.Application.Authorization
{
    internal static class OPERATIONS
    {
        internal static class USER
        {
            internal const string CREATE = "User.Create";
            internal const string READ = "User.Read";
            internal const string EDIT = "User.Edit";
            internal const string DELETE = "User.Delete";
        }

        internal static class ORDER
        {
            internal const string CREATE = "Order.Create";
            internal const string READ = "Order.Read";
            internal const string EDIT = "Order.Edit";
            internal const string DELETE = "Order.Delete";
        }
    }
}