---
template: post
slug: jackolert
title: Jack-o'-lert
description: A detailed overview on programmatically sending templated emails with Python and Jinja2.
date: 2020-08-20
---

I created this repository to serve as a detailed overview on programmatically sending emails. I'm using Python here because I've noticed that most languages seem to rely on a third party module for this type of stuff, and I find them to be overly verbose for such a simple task. Python has the excellent [smtplib](https://docs.python.org/3/library/smtplib.html) built in and we can leverage [Jinja2](https://jinja.palletsprojects.com/en/2.11.x/templates/) templates to send pretty much anything we would ever need.

## Setup

Start by cloning the repository.

```sh
git clone https://github.com/jokocide/jack-o-lert.git
```

Move to the jack-o-lert directory and create a virtual environment.

```sh
python3 -m venv venv
```

Activate the virtual environment.

```sh
# Windows
.\venv\Scripts\activate

# Unix
source venv/bin/activate
```

Confirm that pip is actually pointing to your virtual environment.

```sh
# Windows
Get-Command pip
# ... C:\Users\jokocide\Code\jack-o-lert\venv\Scripts\pip.exe

# Unix
which pip
# ... /users/jokocide/code/jack-o-lert/venv/bin/pip
```

Install the project dependencies.

```sh
python3 -m pip install -r requirements.txt
```

This project is set up with some example functionality. You can open up <span class="refer-code">main.py</span> and see how we are making a GET request to an API, processing the data with a few functions and then injecting our data into the Jinja2 template before we send the email with smtplib. This is all kept pretty simple and thanks to Python's standard library we don't really need to rely on using a ton of third party modules to get this done. You can remove any remaining module imports that you don't need, but I've kept them pretty sparse to be applicable to most use cases.

Go ahead and open up <span class="refer-code">main.py</span>. You'll see that the document includes docstrings to walk you through configuring the files that we need to get this going, but here is an overview:

1. Rename the <span class="refer-code">jack-o-lert/jackolert/dummy_env.py</span> file to <span class="refer-code">env.py</span>. Update the values.
2. Review the template file at <span class="refer-code">jack-o-lert/jackolert/template/template.py</span>
3. <span class="refer-code">main.py</span> includes example functions to demonstrate making an API call. The <span class="refer-code">sendMail</span> function details using smptlib and Jinja2, so you might want to pay attention to that one.

> Seeing magic variables? don't forget that we are importing from <span class="refer-code">template.py</span> & <span class="refer-code">env.py</span>!

After everything is said and done, you can try firing off a test email to your own address by targeting yourself in the receiving_address field in env.py. You can also send emails to multiple people by specifying it in the following format:

```py
receiving_address = "person1@outlook.com, person2@outlook.com"
```

Feel free to [contact](mailto:jokobox@outlook.com) me if have questions or comments.