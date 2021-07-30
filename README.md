# 🗡 Dagger

Dagger is a static site generator built on .NET.

~~> This project is currently under construction, but look for Dagger on [NuGet](https://www.nuget.org/) soon!~~

> Dagger has been released! improvements are coming soon! https://www.nuget.org/packages/Jokocide.Dagger/

To start using Dagger, you'll need to have the [**.NET SDK**](https://dotnet.microsoft.com/download) installed.

The .NET SDK is a set of libraries and tools that allow developers to create and use .NET applications and libraries:

- The .NET CLI.
- .NET libraries and runtime.
- The dotnet driver - *used to install tools like Dagger!*

If you aren't familiar with the ecosystem, .NET can install and run command-line tools just as you would with NPM on the Node.js side. 

With the .NET SDK installed, go ahead and install Dagger:

`dotnet tool install Jokocide.Dagger -g`

Dagger will now be available. Call it from anywhere with the `dagger` command.

## How does it work?

Dagger sites consist of a **resources** and **site** folder. The **resources** folder contains content as written by you, and using `dagger build` will process the **resources** to generate the contents of the **site** folder. You can preview the site with `dagger serve` before hosting it somewhere like Netlify or Github Pages.

## How do I use it?

After installing the tool, try using `dagger help` to get started. I would recommend creating a new directory, moving into that directory and then using `dagger new` to create a new example site. You can dig around in the **resources** folder and then use `dagger serve` to view the resulting site. The example has already been built for you after calling `dagger new`, so no need to use `dagger build` before serving the site for the first time.

If you decide to modify any of the files in the resources folder, you will need to use `dagger build` to regenerate the site. I would recommend using two terminal tabs here -- one to serve the project with `dagger serve` and one to 'watch' the contents of the resources folder for changes and automatically recompile the site when a change is detected, which can be accomplished with `dagger watch`.
