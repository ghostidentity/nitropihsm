# NitroKey HSM 2 Integration with Raspberry Pi
This guide demonstrates how to leverage the advanced features of the NitroKey HSM 2 to protect sensitive data on a Raspberry Pi device.

# Prerequisites
## Hardware Requirements:
- Nitrokey HSM 2
- Raspberry Pi 3 Model B
## Software Requirements:
- A Raspberry Pi running Raspbian OS
Internet connection for package installation

## Step 1: Initialize the HSM (If Not Already Done)
Important: Running the command sc-hsm-tool --initialize will reset the device and erase all existing keys. Only execute this if you have recently procured the device, or if you need to initialize it for the first time. If the device is already initialized, skip this step.

## To initialize the device, run the following command:

sc-hsm-tool --initialize --reader "0" --so-pin SO_PIN --pin USER_PIN
SO_PIN: The Security Officer PIN (default: 1234567890123456)
USER_PIN: A newly generated User PIN (e.g., 654321098765)
Example:

```
sc-hsm-tool --initialize --reader "0" --so-pin 1234567890123456 --pin 654321098765
```

## Step 2: Verify Initialization
After initialization, verify the device status using the following commands:

```
pkcs15-tool --list-pin
pkcs15-tool --list-keys
```
## Step 3: Install Required Libraries on Raspberry Pi
Install the necessary libraries on your Raspberry Pi 3 Model B:

```
sudo apt update
sudo apt install -y opensc
sudo apt-get install libengine-pkcs11-openssl
```

## Step 4: Generate Key Pairs
Generate an RSA key pair for use with the HSM:

```
pkcs11-tool --module /usr/lib/arm-linux-gnueabihf/opensc-pkcs11.so -l --pin 654321098765 --keypairgen --key-type rsa:2048 --id 02
```

```
pkcs15-tool --read-public-key "02" --reader 0 --pin 654321098765 --output public_key.pem
```

Note: The above code generates an RSA key pair. Ensure that the correct Key ID and Slot Number are used in config.xml. Also, move the generated public_key.pem file to the keys folder of your project directory. This file will be mapped to the respective user.

## Step 5: Run the program
Once the key pair is generated and the configuration is set, run the program. The first time you run it, you will need to generate a hashed password for each user. This can be done via the /hasher endpoint on the server.

```
chmod +x nitropi
sudo nohup ./nitropi &
```

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
You can use the public_key.pem generated on Step 4 within your own program to encrypt text data. The encrypted data should be base64-encoded before sending it to the /decrypt endpoint for decryption. Check the "external" folder/

## Notes:
Replace {raspberrypi_ip} with the actual IP address of your Raspberry Pi.
Ensure the Raspberry Pi has a static IP address for ease of access during integration.

## Known Limitations
The current implementation only supports RSA-based encryption.

## Revision Logs
- v1: Initial working encryption
- v2: Added support for concurrency, process optimization, and port adjustment to :80 (01/04/2025)