using System.ComponentModel.DataAnnotations;
using CH.CleanArchitecture.Core.Application;

namespace CH.CleanArchitecture.Presentation.Web.Validators.Internals
{
    internal class LocalizedValidationContext
    {
        internal ILocalizationService Localizer { get; }

        internal ValidationContext Context { get; }

        internal LocalizedValidationContext(ILocalizationService localizer, object instance) {
            Localizer = localizer;
            Context = new ValidationContext(instance);
        }

        internal LocalizedValidationContext(ILocalizationService localizer, ValidationContext context) {
            Localizer = localizer;
            Context = context;
        }
    }
}
