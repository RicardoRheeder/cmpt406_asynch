using System.Security.Cryptography;
using System.Text;

public static class StringEncryption {
    public static string EncryptString(string inputString) {
        StringBuilder encryptedString = new StringBuilder();

        HashAlgorithm alg = SHA256.Create();
        byte[] encryptedBytes = alg.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        foreach( byte b in encryptedBytes) {
            encryptedString.Append(b.ToString("X2"));
        }

        return encryptedString.ToString();
    }
}
