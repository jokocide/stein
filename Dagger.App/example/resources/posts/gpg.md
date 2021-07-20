---
template: post
slug: gpg
title: GPG: Crash Course
description: A very brief reference guide on encrypting and decrypting files with GPG.
date: 2020-12-04
---
## Key Creation

To begin, you need to create your keys.

```sh
gpg --full-gen-key
```

Follow the prompts and your keys will be created and added to your keyring -- unless you have a reason not to, go with the defaults.

## Encryption

In our current directory, we have a text file called 'message' containing a secret.

```sh
> ls -l
total 16
-rw-r--r--  1 jkoenig  staff    22 Dec  4 09:33 message

> cat message
37.2431° N, 115.7930° W
```

To encrypt our message, we use the <span class="refer-code">-r</span> option and specify the email that was used during key creation. The <span class="refer-code">-e</span> option allows you to target the file.

```sh
> gpg -r jokobox@outlook.com -e message
> ls -l
total 24
-rw-r--r--  1 jkoenig  staff    22 Dec  4 09:33 message
-rw-r--r--  1 jkoenig  staff   488 Dec  4 09:37 message.gpg
```

You can see a new file <span class="refer-code">message.gpg</span> now exists, which represents our encrypted message.

Remember that deleting the original unencrypted file with your GUI on Windows/MacOS or <span class="refer-code">rm</span> on the command line [will not destroy the data](https://en.wikipedia.org/wiki/Data_remanence) and you should never rely on this when you are working with real, sensitive information!

## Decryption

To decrypt our file and make it human readable again, the <span class="refer-code">-d</span> option is used.

```sh
> gpg -d message.gpg

gpg: encrypted with 3072-bit RSA key, ID xxxxxxxxxxxx, created 2020-12-02
"Jon Koenig <jokobox@outlook.com>"

37.2431° N, 115.7930° W
```

This will prompt you to enter the key's password. If you do so, the contents of the file will be sent to [stdout](https://en.wikipedia.org/wiki/Standard_streams) by default. Your password will be cached to make decrypting multiple files at once a less painful process, but this can be [changed](https://forums.linuxmint.com/viewtopic.php?t=254042).