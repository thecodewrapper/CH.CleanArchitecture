using Ardalis.GuardClauses;

namespace CH.CleanArchitecture.Core.Domain
{
    public class PhoneNumber
    {
        #region Fields

        public string Value { get; private set; }

        #endregion

        #region Constructor

        private PhoneNumber() { }
        public PhoneNumber(string number) {

            Guard.Against.NullOrWhiteSpace(number, nameof(number));

            Value = number;
        }

        #endregion
    }
}
