using System.Globalization;
using System.Reflection;
using CH.CleanArchitecture.Core.Application;
using Microsoft.Extensions.Localization;

namespace CH.CleanArchitecture.Infrastructure.Resources
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IStringLocalizer _localizer;

        public LocalizationService(IStringLocalizerFactory factory) {
            var type = typeof(SharedResources);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
            _localizer = factory.Create("SharedResources", assemblyName.Name);
        }

        public LocalizedString this[string key] => _localizer[key];

        public LocalizedString GetLocalizedString(string key) {
            return _localizer[key];
        }

        public LocalizedString GetCulturedLocalizedString(string key, string culture) {
            CultureInfo cultureInfo = new CultureInfo(culture);
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;
            return GetLocalizedString(key);
        }

        public string GetLocalizedString(string key, params string[] parameters) {
            return string.Format(_localizer[key], parameters);
        }
    }
}
