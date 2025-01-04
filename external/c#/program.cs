using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace NitroIntegration
{
	internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("NitroKey HSM 2 Integration on Raspbery 3 Model B");

            // Path to your public key (PEM format)
            string publicKeyPath = "public_key.pem";
            
            // Text to encrypt
            string plainText = "Hello, this is a test encryption!";
            
            // Load the public key
            RSA publicKey = LoadPublicKey(publicKeyPath);

            // Encrypt the plain text using RSA and PKCS#1.5 padding
            byte[] encryptedBytes = EncryptText(plainText, publicKey);

            // Convert the encrypted bytes to Base64 string
            string base64EncryptedText = Convert.ToBase64String(encryptedBytes);

            Console.WriteLine("Encrypted Base64 text: ");
            Console.WriteLine(base64EncryptedText);

            // Send the encrypted data to the endpoint
            await SendEncryptedDataAsync(base64EncryptedText);
        }

        // Load the public key from a PEM file
        static RSA LoadPublicKey(string publicKeyPath)
        {
            string publicKeyPem = File.ReadAllText(publicKeyPath);
            RSA rsa = RSA.Create();
            rsa.ImportFromPem(publicKeyPem.ToCharArray());

            return rsa;
        }

        // Encrypt the plain text using RSA with PKCS#1.5 padding
        static byte[] EncryptText(string plainText, RSA publicKey)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedBytes = publicKey.Encrypt(inputBytes, RSAEncryptionPadding.Pkcs1);

            return encryptedBytes;
        }

        // Send the encrypted data to the server via POST with Basic Authentication
        static async Task SendEncryptedDataAsync(string encryptedText)
        {
            using (var client = new HttpClient())
            {
                // Set up the Basic Authentication header
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes("zerocore:letmein")));

                // Prepare the form data as URL-encoded (same as your Go example)
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("input", encryptedText)
                });

                // Set the content type to application/x-www-form-urlencoded
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                // Send the POST request to the server
                HttpResponseMessage response = await client.PostAsync("http://192.168.88.251/decrypt", content);

                // Check the response status
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Server Response:");
                    Console.WriteLine(responseContent);
                }
                else
                {
                    Console.WriteLine("Error: " + response.StatusCode);
                }
            }
        }
    }
}
