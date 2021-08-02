---
template: detail
title: Pages
description: Creating new pages and utilizing your collections.
date: 2021-08-01
---
A page is a document that isn't generated from Markdown or JSON, but exists on its own and is written in plain HTML. These pages often make up areas of a site 
that are written once and used to host more dynamic content, such as blog posts. For example, the [index](/) of this site is considered a page and is generated 
from the document located at resources/pages/index.hbs.

When dagger examines a file that it finds within resources/pages it will eventually place the processed file in a directory similar to where it was found
in resources, but it will be placed in a directory named after the file, with the file itself now being called index.html. This makes it possible to navigate
to the new page by typing its name. For** example, a page like resources/pages/about.hbs will end up at site/about/index.html.