---
template: detail
title: Pages
description: Creating new pages and utilizing your collections.
date: 2021-08-01
---
A page is a document written in plain HTML and is most often used to host other more dynamic content, such as posts in the context of a blog. 
For example, the [index](/) of this site is considered a page and is generated from the document located at 'project/resources/pages/index.hbs'.

When Dagger examines a file that it finds within 'project/resources/pages' it will eventually place the processed file in a directory similar to where it was found, 
with just a couple of small changes. The name of the file will become the name of a new directory within 'project/site' and the file itself will now be titled index.html
and placed within that new directory.

A document at 'project/resources/pages/about.hbs' will end up at 'project/site/about/index.html' and when the site is live, you could reach that page by navigating to
domain.com/about -- you get a clean URL without sacrificing a simple project layout during development.