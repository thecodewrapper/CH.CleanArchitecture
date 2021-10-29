// This source code is fork from https://github.com/dotnet/corefx/blob/master/src/System.ComponentModel.Annotations/src/System/ComponentModel/DataAnnotations/Validator.cs
// The .NET Foundation licenses the original file of this forked file to you under the MIT license.
// See the LICENSE file for the original file of this forked file: https://github.com/dotnet/corefx/blob/master/LICENSE.TXT

using System.ComponentModel.DataAnnotations;

namespace CH.CleanArchitecture.Presentation.Web.Validators.Internals
{
    internal class ValidationError
    {
        private readonly object _value;

        private readonly ValidationAttribute _validationAttribute;

        internal ValidationResult ValidationResult { get; }

        internal ValidationError(ValidationAttribute validationAttribute, object value, ValidationResult validationResult) {
            _validationAttribute = validationAttribute;
            ValidationResult = validationResult;
            _value = value;
        }
    }
}
