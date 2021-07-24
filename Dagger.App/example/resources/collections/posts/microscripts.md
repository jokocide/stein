---
template: post
slug: microscripts
title: Microscripts
description: My Powershell/BASH configuration. These are essentially just miniature scripts that make common tasks (almost) 3 seconds faster for me.
date: 2020-07-14
---
## INSTALLATION

Clone the [repository.](https://github.com/jokocide/microscripts.git) Review the files and remove the directories that are not relevant to you. (ie 'pwsh' on unix systems)

The bash and pwsh directorieds include a <span class="refer-code">.bashrc</span> and <span class="refer-code">profile.ps1</span> respectively, but the functions have their own individual files as well. I sometimes like to keep them in a separate directory, add it to my [path](https://linuxize.com/post/how-to-add-directory-to-path-in-linux/) and call them that way rather than pollute my configuration file with a ton of functions.

```sh
git clone https://github.com/jokocide/microscripts.git
```

I focus on developing the scripts in Powershell and BASH. They should all perform exactly the same barring any bugs, so you should delete the directories that are not relevant to you. Keeping everything together in one repository is easier for me, so I apologize for any inconvenience that might cause you.

## USAGE

If you are keeping the functions in a directory rather than adding them directly to your profile, you should be able to start typing the name of the function you are after to get some nice tab completion. This is assuming you didn't forget to add the directory to your path as described above, and you might also need to log out/in to get this working.

If you've added some of the functionality directly to your profile, don't forget to [source!](https://linuxize.com/post/bash-source-command/)