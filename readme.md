# NitroKey HSM 2 Integration with Raspberry Pi
This guide demonstrates how to leverage the advanced features of the NitroKey HSM 2 to protect sensitive data on a Raspberry Pi device.

## Supported Functions
- Encrypt
- Decrypt
- Sign
- Verify

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

Note: The above code generates an RSA key pair. Ensure that the correct Key ID and Slot Number are used in config.xml. Also, move the generated public_key.pem file to the keys folder. Each user will use spefic public key for encryption.


## Step 5: Run the program
Once the key pair is generated and the configuration is set, run the program. The first time you run it, you will need to generate a hashed password for each user. This can be done via the /hasher endpoint on the server.

```
chmod +x nitropi
sudo nohup ./nitropi &
```

## Changes
- 1/11/2025 now supports Sign and Verify, it uses RSA private key to sign the file. Likewise, you can use Verify to verify the signature.
- 1/11/2025 fix bug on client side and optimization
- 1/12/2025 updated client to be compatible with ngrok, u need to use ngrok tcp, then paste in server:port  (exclude TCP)
- 1/12/2025 updated server with option to enable ngrok. To enable, just update teh config.xml. When you run the server on background, the ngrok address will not be visible, so you need to navigate to https://dashboard.ngrok.com/endpoints?sortBy=createdAt&orderBy=desc

## Known Limitations
- The current implementation only supports RSA-based encryption.
- When you copy the output base64 from encryption and signature result, it retains the new line when you paste it, u can use notepad and unselect the wordwrap option.

## Ngrok Integration on Raspberry PI 3
1. Create Ngrok account
2. Navigate to https://dashboard.ngrok.com/
3. Create Bot-User, you can name it Raspberry pi: https://dashboard.ngrok.com/bot-users,
4. Once created, click on the bot, and generate authtoken
5. On config.xml, enable it, then add the bot_user_auth_token.
6. If you have paid subscription, you can update the tcpaddres, otherwise, set it empty.
```

## Notes 
- The server executable expects the client.xml file to be in the same folder. It also expects the public_keys to be present as defined in client.xml. Make sure to use the appropriate executable based on your OS and the correct OpenSC library.
- Do not use public_key.pem for encryption. You must generate a new RSA key pair and download the public key from it.
- DO NOT USE the initialize command unless you have recently purchased the device. This command will reset the device’s keys.
- If you encrypt or sign any data, remember to securely store the ciphertext (in base64 format). 
- When running the server on background and with ngrok option enabled, the server endpoint will not be visible, navigate to https://dashboard.ngrok.com/endpoints?sortBy=createdAt&orderBy=desc to obtain it.
- Ngrok Authtoken: You can use the token from Bot Users https://dashboard.ngrok.com/bot-users, if not u can obtain it directly from  https://dashboard.ngrok.com/authtokens
- Ngrok TcpAdress: Optional, leave empty, unless you are using paid subscription. https://dashboard.ngrok.com/tcp-addresses
```
sc-hsm-tool --initialize --reader "0" --so-pin 1234567890123456 --pin 654321098765
```

## Disclaimer
- An effort has been made to ensure the program is reliable, but use at your own risk. If the device malfunctions, or if you forget to secure the ciphertext or mistakenly configure your device, it falls beyond the developer's responsibility.

## Author
- Mark Tagab