using System.Reflection;

namespace CH.CleanArchitecture.Tests
{
    public static class ObjectHelper
    {
        /// <summary>
        /// Tries to set a property's value of an object using reflection.
        /// </summary>
        /// <param name="obj">The object whose property value you want to set.</param>
        /// <param name="propertyName">The name of the property you want to set.</param>
        /// <param name="value">The value you want to set the property to.</param>
        /// <returns>true if the property was set successfully, false otherwise.</returns>
        public static void SetProperty(object obj, string propertyName, object value) {
            if (obj == null) {
                throw new ArgumentNullException(nameof(obj));
            }

            if (string.IsNullOrEmpty(propertyName)) {
                throw new ArgumentException("Property name can't be null or empty.", nameof(propertyName));
            }

            PropertyInfo property = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

            if (property != null) {
                try {
                    property.SetValue(obj, value);
                }
                catch (Exception) {
                    // Handle any exception that might occur during property setting.
                    // For now, we're simply swallowing the exception and returning false.
                    // In a real-world application, you might want to log this exception or handle it differently.
                }
            }
        }
    }
}
