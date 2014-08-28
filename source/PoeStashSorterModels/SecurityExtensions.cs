using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace POEStashSorterModels
{
    /// <span class="code-SummaryComment"><summary></span>
    /// Provides extension methods that deal with
    /// string encryption/decryption and
    /// <span class="code-SummaryComment"><see cref="SecureString"/> encapsulation.</span>
    /// <span class="code-SummaryComment"></summary></span>
    public static class SecurityExtensions
    {
        /// <span class="code-SummaryComment"><summary></span>
        /// Specifies the data protection scope of the DPAPI.
        /// <span class="code-SummaryComment"></summary></span>
        private const DataProtectionScope Scope = DataProtectionScope.CurrentUser;

        /// <span class="code-SummaryComment"><summary></span>
        /// Encrypts a given password and returns the encrypted data
        /// as a base64 string.
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="plainText">An unencrypted string that needs</span>
        /// to be secured.<span class="code-SummaryComment"></param></span>
        /// <span class="code-SummaryComment"><returns>A base64 encoded string that represents the encrypted</span>
        /// binary data.
        /// <span class="code-SummaryComment"></returns></span>
        /// <span class="code-SummaryComment"><remarks>This solution is not really secure as we are</span>
        /// keeping strings in memory. If runtime protection is essential,
        /// <span class="code-SummaryComment"><see cref="SecureString"/> should be used.</remarks></span>
        /// <span class="code-SummaryComment"><exception cref="ArgumentNullException">If <paramref name="plainText"/></span>
        /// is a null reference.<span class="code-SummaryComment"></exception></span>
        public static string Encrypt(this string plainText)
        {
            if (plainText == null) throw new ArgumentNullException("plainText");

            //encrypt data
            var data = Encoding.Unicode.GetBytes(plainText);
            byte[] encrypted = ProtectedData.Protect(data, null, Scope);

            //return as base64 string
            return Convert.ToBase64String(encrypted);
        }

        /// <span class="code-SummaryComment"><summary></span>
        /// Decrypts a given string.
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="cipher">A base64 encoded string that was created</span>
        /// through the <span class="code-SummaryComment"><see cref="Encrypt(string)"/> or</span>
        /// <span class="code-SummaryComment"><see cref="Encrypt(SecureString)"/> extension methods.</param></span>
        /// <span class="code-SummaryComment"><returns>The decrypted string.</returns></span>
        /// <span class="code-SummaryComment"><remarks>Keep in mind that the decrypted string remains in memory</span>
        /// and makes your application vulnerable per se. If runtime protection
        /// is essential, <span class="code-SummaryComment"><see cref="SecureString"/> should be used.</remarks></span>
        /// <span class="code-SummaryComment"><exception cref="ArgumentNullException">If <paramref name="cipher"/></span>
        /// is a null reference.<span class="code-SummaryComment"></exception></span>
        public static string Decrypt(this string cipher)
        {
            if (cipher == null) throw new ArgumentNullException("cipher");

            //parse base64 string
            byte[] data = Convert.FromBase64String(cipher);

            //decrypt data
            byte[] decrypted = ProtectedData.Unprotect(data, null, Scope);
            return Encoding.Unicode.GetString(decrypted);
        }

        /// <span class="code-SummaryComment"><summary></span>
        /// Encrypts the contents of a secure string.
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="value">An unencrypted string that needs</span>
        /// to be secured.<span class="code-SummaryComment"></param></span>
        /// <span class="code-SummaryComment"><returns>A base64 encoded string that represents the encrypted</span>
        /// binary data.
        /// <span class="code-SummaryComment"></returns></span>
        /// <span class="code-SummaryComment"><exception cref="ArgumentNullException">If <paramref name="value"/></span>
        /// is a null reference.<span class="code-SummaryComment"></exception></span>
        public static string Encrypt(this SecureString value)
        {
            if (value == null) throw new ArgumentNullException("value");

            IntPtr ptr = Marshal.SecureStringToCoTaskMemUnicode(value);
            try
            {
                char[] buffer = new char[value.Length];
                Marshal.Copy(ptr, buffer, 0, value.Length);

                byte[] data = Encoding.Unicode.GetBytes(buffer);
                byte[] encrypted = ProtectedData.Protect(data, null, Scope);

                //return as base64 string
                return Convert.ToBase64String(encrypted);
            }
            finally
            {
                Marshal.ZeroFreeCoTaskMemUnicode(ptr);
            }
        }

        /// <span class="code-SummaryComment"><summary></span>
        /// Decrypts a base64 encrypted string and returns the decrpyted data
        /// wrapped into a <span class="code-SummaryComment"><see cref="SecureString"/> instance.</span>
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="cipher">A base64 encoded string that was created</span>
        /// through the <span class="code-SummaryComment"><see cref="Encrypt(string)"/> or</span>
        /// <span class="code-SummaryComment"><see cref="Encrypt(SecureString)"/> extension methods.</param></span>
        /// <span class="code-SummaryComment"><returns>The decrypted string, wrapped into a</span>
        /// <span class="code-SummaryComment"><see cref="SecureString"/> instance.</returns></span>
        /// <span class="code-SummaryComment"><exception cref="ArgumentNullException">If <paramref name="cipher"/></span>
        /// is a null reference.<span class="code-SummaryComment"></exception></span>
        public static SecureString DecryptSecure(this string cipher)
        {
            if (cipher == null) throw new ArgumentNullException("cipher");

            //parse base64 string
            byte[] data = Convert.FromBase64String(cipher);

            //decrypt data
            byte[] decrypted = ProtectedData.Unprotect(data, null, Scope);

            SecureString ss = new SecureString();

            //parse characters one by one - doesn't change the fact that
            //we have them in memory however...
            int count = Encoding.Unicode.GetCharCount(decrypted);
            int bc = decrypted.Length / count;
            for (int i = 0; i < count; i++)
            {
                ss.AppendChar(Encoding.Unicode.GetChars(decrypted, i * bc, bc)[0]);
            }

            //mark as read-only
            ss.MakeReadOnly();
            return ss;
        }

        /// <span class="code-SummaryComment"><summary></span>
        /// Wraps a managed string into a <span class="code-SummaryComment"><see cref="SecureString"/> </span>
        /// instance.
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="value">A string or char sequence that </span>
        /// should be encapsulated.<span class="code-SummaryComment"></param></span>
        /// <span class="code-SummaryComment"><returns>A <see cref="SecureString"/> that encapsulates the</span>
        /// submitted value.<span class="code-SummaryComment"></returns></span>
        /// <span class="code-SummaryComment"><exception cref="ArgumentNullException">If <paramref name="value"/></span>
        /// is a null reference.<span class="code-SummaryComment"></exception></span>
        public static SecureString ToSecureString(this IEnumerable<char> value)
        {
            if (value == null) throw new ArgumentNullException("value");

            var secured = new SecureString();

            var charArray = value.ToArray();
            for (int i = 0; i < charArray.Length; i++)
            {
                secured.AppendChar(charArray[i]);
            }

            secured.MakeReadOnly();
            return secured;
        }

        /// <span class="code-SummaryComment"><summary></span>
        /// Unwraps the contents of a secured string and
        /// returns the contained value.
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="value"></param></span>
        /// <span class="code-SummaryComment"><returns></returns></span>
        /// <span class="code-SummaryComment"><remarks>Be aware that the unwrapped managed string can be</span>
        /// extracted from memory.<span class="code-SummaryComment"></remarks></span>
        /// <span class="code-SummaryComment"><exception cref="ArgumentNullException">If <paramref name="value"/></span>
        /// is a null reference.<span class="code-SummaryComment"></exception></span>
        public static string Unwrap(this SecureString value)
        {
            if (value == null) throw new ArgumentNullException("value");

            IntPtr ptr = Marshal.SecureStringToCoTaskMemUnicode(value);
            try
            {
                return Marshal.PtrToStringUni(ptr);
            }
            finally
            {
                Marshal.ZeroFreeCoTaskMemUnicode(ptr);
            }
        }

        /// <span class="code-SummaryComment"><summary></span>
        /// Checks whether a <span class="code-SummaryComment"><see cref="SecureString"/> is either</span>
        /// null or has a <span class="code-SummaryComment"><see cref="SecureString.Length"/> of 0.</span>
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="value">The secure string to be inspected.</param></span>
        /// <span class="code-SummaryComment"><returns>True if the string is either null or empty.</returns></span>
        public static bool IsNullOrEmpty(this SecureString value)
        {
            return value == null || value.Length == 0;
        }

        /// <span class="code-SummaryComment"><summary></span>
        /// Performs bytewise comparison of two secure strings.
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="value"></param></span>
        /// <span class="code-SummaryComment"><param name="other"></param></span>
        /// <span class="code-SummaryComment"><returns>True if the strings are equal.</returns></span>
        public static bool Matches(this SecureString value, SecureString other)
        {
            if (value == null && other == null) return true;
            if (value == null || other == null) return false;
            if (value.Length != other.Length) return false;
            if (value.Length == 0 && other.Length == 0) return true;

            IntPtr ptrA = Marshal.SecureStringToCoTaskMemUnicode(value);
            IntPtr ptrB = Marshal.SecureStringToCoTaskMemUnicode(other);
            try
            {
                //parse characters one by one - doesn't change the fact that
                //we have them in memory however...
                byte byteA = 1;
                byte byteB = 1;

                int index = 0;
                while (((char)byteA) != '\0' && ((char)byteB) != '\0')
                {
                    byteA = Marshal.ReadByte(ptrA, index);
                    byteB = Marshal.ReadByte(ptrB, index);
                    if (byteA != byteB) return false;
                    index += 2;
                }

                return true;
            }
            finally
            {
                Marshal.ZeroFreeCoTaskMemUnicode(ptrA);
                Marshal.ZeroFreeCoTaskMemUnicode(ptrB);
            }
        }
    }
}
