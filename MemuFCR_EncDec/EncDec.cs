
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace MemuFCR_EncDec
{
    public class EncDec
    {        

        private string keyEncDec = "p@gIb1gUbp@CC2019";        

        public string InputData { get; set; }
        public string OutputData { get; set; }
        public string ErrorMessage { get; set; }

        public EncDec()
        {

        }

        public EncDec(string keyEncDec)
        {
            if(keyEncDec!="")this.keyEncDec = keyEncDec;            
        }

        public bool EncryptData()
        {
            try
            {
                if (!Validations(InputData)) return false;

                byte[] passwordBytes = GetPasswordBytes();
                OutputData = AES.Encrypt(InputData, passwordBytes);

                return true;
            }
            catch(Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
             }
        }

        public bool DecryptData()
        {
            try
            {
                if (!Validations(InputData)) return false;

                byte[] passwordBytes = GetPasswordBytes();
                OutputData = AES.Decrypt(InputData, passwordBytes);

                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }
        }

        private bool Validations(string inputData)
        {
            if (keyEncDec.Length < 16)
            {
                ErrorMessage = "Key length is invalid";
                return false;
            }

            if (inputData.Trim() == "")
            {
                ErrorMessage = "Input data is empty";
                return false;
            }

            return true;
        }

        private byte[] GetPasswordBytes()
        {
            // The real password characters is stored in System.SecureString
            // Below code demonstrates on converting System.SecureString into Byte[]
            // Credit: http://social.msdn.microsoft.com/Forums/vstudio/en-US/f6710354-32e3-4486-b866-e102bb495f86/converting-a-securestring-object-to-byte-array-in-net

            byte[] ba = null;

            if (this.keyEncDec.Length == 0)
                ba = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            else
            {
                SecureString _secureEntry = new SecureString();

                foreach (Char _char in this.keyEncDec) _secureEntry.AppendChar(_char);
               
                // Convert System.SecureString to Pointer
                IntPtr unmanagedBytes = System.Runtime.InteropServices.Marshal.SecureStringToGlobalAllocAnsi(_secureEntry);
                try
                {
                    // You have to mark your application to allow unsafe code
                    // Enable it at Project's Properties > Build
                    unsafe
                    {
                        byte* byteArray = (byte*)unmanagedBytes.ToPointer();

                        // Find the end of the string
                        byte* pEnd = byteArray;
                        while (*pEnd++ != 0) { }
                        // Length is effectively the difference here (note we're 1 past end) 
                        int length = (int)((pEnd - byteArray) - 1);

                        ba = new byte[length];

                        for (int i = 0; i < length; ++i)
                        {
                            // Work with data in byte array as necessary, via pointers, here
                            byte dataAtIndex = *(byteArray + i);
                            ba[i] = dataAtIndex;
                        }
                    }
                }
                finally
                {
                    // This will completely remove the data from memory
                    Marshal.ZeroFreeGlobalAllocAnsi(unmanagedBytes);
                }
            }

            return System.Security.Cryptography.SHA256.Create().ComputeHash(ba);
        }

    }
  
}
