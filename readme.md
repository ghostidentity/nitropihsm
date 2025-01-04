# NitroPI HSM Integration with Raspberry Pi
This guide allow you to take advantage of NitroKey HSM 2 advance features to protect sensitive data on Raspberry Pi device.

## Hardware Required:

- Nitrokey HSM 2
- Raspberry Pi 3 Model B

## Initialize the HSM, skip if you already initialize it already.
IMPORTANT!!
the command: sc-hsm-tool --initialize will reset the device which will remove all existing keys, only do this if you recently procured the device. otherwise, you can skip it.

- sc-hsm-tool --initialize --reader "0" --so-pin SO_PIN --pin USER_PIN
- e.g.  sc-hsm-tool --initialize --reader "0" --so-pin 1234567890123456 --pin 654321098765
- note the : 1234567890123456 is the SO pin DEFAULT, the 654321098765 is the newly generated user pin.

## Verification
- pkcs15-tool --list-pin
- pkcs15-tool --list-public-keys --reader 0 --pin 654321098765
- pkcs15-tool --list-keys


## Install libraries on Raspberry Pi 3 Model B

```
sudo apt install -y opensc
sudo apt-get install libengine-pkcs11-openssl
```

## Generate KeyPairs
```
pkcs11-tool --module /usr/lib/arm-linux-gnueabihf/opensc-pkcs11.so -l --pin 654321098765 --keypairgen --key-type rsa:2048 --id 02
pkcs15-tool --read-public-key "02" --reader 0 --pin 654321098765 --output public_key.pem
```
- Note: the above code will generate rsa key pair, u need to use the ID and slot number on config.xml, u also need to move the public key to keys folder. It's important to  map the correct public key for each user
- Afterward, run the executable:   (first run will require you to generate the password for each user, which can be generated at http://192.168.88.251:8080/hasher. Its ideal to set the raspberry PI to static.)
```
chmod +x nitropi
./nitropi
```
-  Once running, use postman to generate the hash value of the pasword you want to use, u need to update the pasword on config.xml associated to the user.
-  Once the config.xml has been modified, restart the program ./nitropi 
-  You can now use the /encrypt and /decrypt endpoint.

## API Endpoints: 

- Encrypt: POST /encrypt (Content-Type: application/x-www-form-urlencoded, input: data)
- Decrypt: POST /decrypt (Content-Type: application/x-www-form-urlencoded, input: data)
- Hash: POST /hasher (Content-Type: application/x-www-form-urlencoded, input: data)

note you can check the screenshots folder.

## Limitation
- The code provided only supports RSA based encryption
