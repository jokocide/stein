---
template: detail
title: Collections
description: Create collections to organize your Markdown content.
date: 2021-08-01
---
A collection is a logical grouping of Markdown content. Collections are located within the 'project/resources/collections' directory. Dagger interprets each 
folder here as an individual collection and all frontmatter within these files is made available by iterating over the collection name within a template. Collection 
files are also compiled and written out to the file system just like [page](/guides/pages/index.html) files.

A collection file located at 'project/resources/collections/guides/pages.md' will end up at 'project/site/guides/pages/index.html'. Dagger projects are intended to be
served at the 'project/site' directory, so this file will be accessible at 'domain.com/guides/pages' after the site is live.

It is essential that collection files include a 'template' key that specifies the name, not including the file extension, of the template file that the body of this 
collection file will be injected into to create a full HTML document.

You can refer to the contents of a collection by the name of the folder that contains the collection files. For example, refer to the 'project/resources/collections' 
directory of this site. It contains a 'guides' collection with three files. The template file located at 'project/resources/pages/index.hbs' iterates over all of these 
items, and in doing so gains access to each file's frontmatter. The template file takes advantage of that to generate a list of items.

As mentioned earlier, iterating over the contents of a collection gets you access to all frontmatter established at the top of each file, but Dagger also includes one 
other 'path' key that you can use to generate links to your content.