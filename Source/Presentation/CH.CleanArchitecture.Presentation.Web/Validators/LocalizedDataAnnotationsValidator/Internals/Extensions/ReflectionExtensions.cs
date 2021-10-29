using System.Reflection;

namespace CH.CleanArchitecture.Presentation.Web.Validators.Internals
{
    internal static class ReflectionExtensions
    {
        internal static bool IsPublic(this PropertyInfo p) {
            if (!(p.GetMethod != null) || !p.GetMethod.IsPublic) {
                if (p.SetMethod != null) {
                    return p.SetMethod.IsPublic;
                }
                return false;
            }
            return true;
        }

    }
}
