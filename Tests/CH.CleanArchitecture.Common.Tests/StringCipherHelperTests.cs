using Xunit;

namespace CH.CleanArchitecture.Common.Tests
{
    public class StringCipherHelperTests
    {
        [Fact]
        public void EncryptWithRandomSalt_Returns_Encrypted_String() {
            string payload = "teststring";
            string encrypted = StringCipherHelper.EncryptWithRandomSalt(payload);

            Assert.NotEqual(payload, encrypted);
        }

        [Fact]
        public void EncryptWithStaticSalt_Returns_Encrypted_String() {
            string payload = "teststring";
            string encrypted = StringCipherHelper.EncryptWithStaticSalt(payload);

            Assert.NotEqual(payload, encrypted);
        }

        [Fact]
        public void Encrypt_Returns_Encrypted_String() {
            string payload = "teststring";
            string encrypted = StringCipherHelper.Encrypt(payload, "somesalt");

            Assert.NotEqual(payload, encrypted);
        }

        [Fact]
        public void DecryptWithRandomSalt_Returns_Decrypted_String() {
            string payload = "teststring";
            string encrypted = StringCipherHelper.EncryptWithRandomSalt(payload);
            string decrypted = StringCipherHelper.DecryptWithRandomSalt(encrypted);

            Assert.Equal(payload, decrypted);
        }

        [Fact]
        public void DecryptWithStaticSalt_Returns_Decrypted_String() {
            string payload = "teststring";
            string encrypted = StringCipherHelper.EncryptWithStaticSalt(payload);
            string decrypted = StringCipherHelper.DecryptWithStaticSalt(encrypted);

            Assert.Equal(payload, decrypted);
        }

        [Fact]
        public void Decrypt_Returns_Decrypted_String() {
            string payload = "teststring";
            string salt = "somesalt";
            string encrypted = StringCipherHelper.Encrypt(payload, salt);
            string decrypted = StringCipherHelper.Decrypt(encrypted, salt);

            Assert.Equal(payload, decrypted);
        }
    }
}