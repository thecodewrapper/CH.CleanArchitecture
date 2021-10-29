// This source code is fork from https://github.com/dotnet/corefx/blob/master/src/System.ComponentModel.Annotations/src/System/ComponentModel/DataAnnotations/Validator.cs
// The .NET Foundation licenses the original file of this forked file to you under the MIT license.
// See the LICENSE file for the original file of this forked file: https://github.com/dotnet/corefx/blob/master/LICENSE.TXT

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace CH.CleanArchitecture.Presentation.Web.Validators.Internals
{
    internal static class LocalizedValidator
    {
        private static readonly ValidationAttributeStore _store = ValidationAttributeStore.Instance;

        internal static bool TryValidateObject(object instance, LocalizedValidationContext validationContext, ICollection<ValidationResult> validationResults, bool validateAllProperties) {
            var result = true;
            var breakOnFirstError = validationResults == null;
            foreach (var objectValidationError in GetObjectValidationErrors(instance, validationContext, validateAllProperties, breakOnFirstError)) {
                result = false;
                validationResults?.Add(objectValidationError.ValidationResult);
            }
            return result;
        }

        internal static bool TryValidateProperty(object value, LocalizedValidationContext validationContext, ICollection<ValidationResult> validationResults) {
            var result = true;
            var breakOnFirstError = validationResults == null;
            var propertyValidationAttributes = _store.GetPropertyValidationAttributes(validationContext.Context);
            foreach (var validationError in GetValidationErrors(value, validationContext, propertyValidationAttributes, breakOnFirstError)) {
                result = false;
                validationResults?.Add(validationError.ValidationResult);
            }
            return result;
        }

        private static IEnumerable<ValidationError> GetObjectValidationErrors(object instance, LocalizedValidationContext validationContext, bool validateAllProperties, bool breakOnFirstError) {
            var list = new List<ValidationError>();
            list.AddRange(GetObjectPropertyValidationErrors(instance, validationContext, validateAllProperties, breakOnFirstError));
            if (list.Any()) return list;

            var typeValidationAttributes = _store.GetTypeValidationAttributes(validationContext.Context);
            list.AddRange(GetValidationErrors(instance, validationContext, typeValidationAttributes, breakOnFirstError));
            if (list.Any()) {
                return list;
            }
            var validatableObject = instance as IValidatableObject;
            if (validatableObject != null) {
                var enumerable = validatableObject.Validate(validationContext.Context);
                if (enumerable != null) {
                    foreach (var item in enumerable.Where(r => r != ValidationResult.Success)) {
                        list.Add(new ValidationError(null, instance, item));
                    }
                    return list;
                }
            }
            return list;
        }

        private static IEnumerable<ValidationError> GetValidationErrors(object value, LocalizedValidationContext validationContext, IEnumerable<ValidationAttribute> attributes, bool breakOnFirstError) {
            var list = new List<ValidationError>();
            var requiredAttribute = attributes.OfType<RequiredAttribute>().FirstOrDefault();
            if (requiredAttribute != null && !TryValidate(value, validationContext, requiredAttribute, out var validationError)) {
                list.Add(validationError);
                return list;
            }
            foreach (var attribute in attributes) {
                if (attribute != requiredAttribute && !TryValidate(value, validationContext, attribute, out validationError)) {
                    list.Add(validationError);
                    if (breakOnFirstError) {
                        return list;
                    }
                }
            }
            return list;
        }

        private static bool TryValidate(object value, LocalizedValidationContext validationContext, ValidationAttribute attribute, out ValidationError validationError) {
            var validationResult = GetLocalizedValidationResult(attribute, value, validationContext);

            if (validationResult != ValidationResult.Success) {
                validationError = new ValidationError(attribute, value, validationResult);
                return false;
            }
            validationError = null;
            return true;
        }

        private static IEnumerable<ValidationError> GetObjectPropertyValidationErrors(object instance, LocalizedValidationContext validationContext, bool validateAllProperties, bool breakOnFirstError) {
            var propertyValues = GetPropertyValues(instance, validationContext);
            var list = new List<ValidationError>();
            foreach (var item in propertyValues) {
                var propertyValidationAttributes = _store.GetPropertyValidationAttributes(item.Key.Context);
                if (validateAllProperties) {
                    list.AddRange(GetValidationErrors(item.Value, item.Key, propertyValidationAttributes, breakOnFirstError));
                }
                else {
                    var requiredAttribute = propertyValidationAttributes.OfType<RequiredAttribute>().FirstOrDefault();
                    if (requiredAttribute != null) {
                        var validationResult = GetLocalizedValidationResult(requiredAttribute, item.Value, item.Key);

                        if (validationResult != ValidationResult.Success) {
                            list.Add(new ValidationError(requiredAttribute, item.Value, validationResult));
                        }
                    }
                }
                if (breakOnFirstError && list.Any()) {
                    return list;
                }
            }
            return list;
        }

        private static ValidationResult GetLocalizedValidationResult(ValidationAttribute attribute, object value, LocalizedValidationContext validationContext) {
            lock (attribute) {
                var displayName = validationContext.Context.DisplayName;
                var errorMessage = attribute.ErrorMessage;
                try {
                    var localizer = validationContext.Localizer;
                    if (!string.IsNullOrEmpty(displayName)) validationContext.Context.DisplayName = localizer[displayName].Value;
                    if (!string.IsNullOrEmpty(errorMessage)) attribute.ErrorMessage = localizer[errorMessage].Value;

                    return attribute.GetValidationResult(value, validationContext.Context);
                }
                finally {
                    attribute.ErrorMessage = errorMessage;
                    if (!string.IsNullOrEmpty(displayName)) validationContext.Context.DisplayName = displayName;
                }
            }
        }

        private static ICollection<KeyValuePair<LocalizedValidationContext, object>> GetPropertyValues(object instance, LocalizedValidationContext validationContext) {
            var enumerable = instance.GetType().GetRuntimeProperties().Where(p => p.IsPublic() && !p.GetIndexParameters().Any());
            var list = new List<KeyValuePair<LocalizedValidationContext, object>>(enumerable.Count());
            foreach (var item in enumerable) {
                var validationContext2 = CreateValidationContext(instance, validationContext);
                validationContext2.Context.MemberName = item.Name;
                if (_store.GetPropertyValidationAttributes(validationContext2.Context).Any()) {
                    list.Add(new KeyValuePair<LocalizedValidationContext, object>(validationContext2, item.GetValue(instance, null)));
                }
            }
            return list;
        }

        private static LocalizedValidationContext CreateValidationContext(object instance, LocalizedValidationContext validationContext) {
            return new LocalizedValidationContext(validationContext.Localizer, new ValidationContext(instance, validationContext.Context, validationContext.Context.Items));
        }
    }
}
