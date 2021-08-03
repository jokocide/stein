---
template: detail
title: Public
description: Where do static files like JavaScript and images go?
date: 2021-08-01
---
Many other static site generators use template helpers that keep track of the names of your static files and allow you to refer to them by that name, but Dagger currently
takes a more simplistic approach by encouraging you to refer to these files with an absolute path.

When a Dagger project is live, any path with a forward slash <span class="hl">/</span> at the very beginning is an absolute path and will be served from the web root. Static files like 
JavaScript, CSS and images are meant to be placed in the projects <span class="hl">project/resources/public</span> directory and are not processed by Dagger beyond being copied over to the <span class="hl">project/site</span> directory during 
a build.

For example, the header partial used by this site is responsible for importing the only 
CSS document required, and it does so with a link element which has an href attribute value of <span class="hl">/public/css/main.css.</span>

