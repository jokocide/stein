---
template: detail
title: Public
description: Where do static files like JavaScript and images go?
date: 2021-08-01
---
Many other static site generators use template helpers that keep track of individual static files and allow you to refer to them by a single keyword, but Dagger currently
takes a more simplistic approach by encouraging you to refer to these files with an absolute path.

When a Dagger project is being hosted, any path with a '/' character at the very beginning is an absolute path from the server's web root. Static files like 
JavaScript, CSS and images are located in 'project/resources/public' and are not processed by Dagger beyond being copied over to the 'project/site' directory during 
a build.

You can always refer to any of your static files by their absolute path, because the location of your public files will never change. 
For example, the header partial used by this site is responsible for importing the only CSS document required, and it does so with a path like '/public/css/main.css'.