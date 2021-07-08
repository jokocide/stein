---
template: "post"
slug: "gpg"
title: "GPG: Crash Course"
description: "A very brief reference guide on encrypting and decrypting files with GPG."
date: "2020-12-04"
---
## Key Creation

To begin, you need to create your keys.

```sh
gpg --full-gen-key
```

Follow the prompts and your keys will be created and added to your keyring -- unless you have a reason not to, go with the defaults.