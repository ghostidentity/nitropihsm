

### Generate User Password Hash:
To generate a password hash, visit the /hasher endpoint on your Raspberry Pi:

Open your browser or Postman and navigate to: http://{raspberrypi_ip}/hasher
Replace {raspberrypi_ip} with the actual IP address of your Raspberry Pi (e.g., http://192.168.88.251/hasher).
Update the password in config.xml:
Once you receive the hashed password, update the Password field for the corresponding user in config.xml.

## API Endpoints Overview

### Encrypt Data:

```
URL: http://{raspberrypi_ip}/encrypt
Method: POST
Content-Type: application/x-www-form-urlencoded
Parameters: input=data
```
### Decrypt Data:
```
URL: http://{raspberrypi_ip}/decrypt
Method: POST
Content-Type: application/x-www-form-urlencoded
Parameters: input=data
```
### Generate Hash:
```
URL: http://{raspberrypi_ip}/hasher
Method: POST
Content-Type: application/x-www-form-urlencoded
Parameters: input=data
```
## External Integration
You can use the public_key.pem generated on Step 4 within your own program to encrypt text data. The encrypted data should be base64-encoded before sending it to the /decrypt endpoint for decryption. Check the "integration" folder/

## Nitropi Utility tool
The nitropi program, is a handy tool that you can bind to system environment variable on windows 11 OS, x64 bit. It can perform encryption and decryption.

## Notes:
Replace {raspberrypi_ip} with the actual IP address of your Raspberry Pi.
Ensure the Raspberry Pi has a static IP address for ease of access during integration.

```
ldconfig -p | grep opensc
```
On windows, its usually C:\Program Files\OpenSC Project\OpenSC\pkcs11\onepin-opensc-pkcs11.dll . If there's no opensc library, u need to install it.
Most importantly, u need to replace the public_key.pem 

## Known Limitations
The current implementation only supports RSA-based encryption.