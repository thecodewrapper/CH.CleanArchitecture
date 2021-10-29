using System;

namespace CH.CleanArchitecture.Infrastructure.Auditing
{
    internal static class AuditUtilities
    {
        public static bool IsAuditDisabled(Type type) {
            var customAttributes = type.GetCustomAttributes(false);

            foreach (var customAttribute in customAttributes) {
                if (customAttribute.GetType() == typeof(NotAuditableAttribute)) {
                    var auditableAttribute = (NotAuditableAttribute)customAttribute;
                    return auditableAttribute.Enabled;
                }
            }

            return false;
        }

        public static bool IsAuditDisabled(Type type, string propertyName) {
            if (propertyName == "Discriminator") //set Discriminator shadow property as non auditable
                return false;

            var customAttributes = type.GetProperty(propertyName).GetCustomAttributes(false);

            foreach (var customAttribute in customAttributes) {
                if (customAttribute.GetType() == typeof(NotAuditableAttribute)) {
                    var auditableAttribute = (NotAuditableAttribute)customAttribute;
                    return auditableAttribute.Enabled;
                }
            }

            return false;
        }
    }
}