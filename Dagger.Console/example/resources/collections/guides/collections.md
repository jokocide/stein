---
template: detail
title: Collections
description: Create collections to organize your Markdown content.
date: 2021-08-01
---
A collection is a logical grouping of Markdown content. Collections are located within a project's resources/collections directory. Dagger interprets each folder here 
as an individual collection and all files within that collection are made available by iterating over the collection within a template. Apart from the frontmatter of 
these collection files being made available for use within a template, they are compiled and written out to the file system just like your 
[page](/collections/guides/pages/index.html) files. 

A collection file located at 'resources/collections/guides/pages.md' will end up at 'site/collections/guides/pages.html' and you can link to that page with an absolute 
path like '/collections/guides/pages.html'.

If you look at this Markdown file as an example (resources/collections/guides/collections.md) you will notice the very first frontmatter item is a 'template' 
key -- this is essential because it tells Dagger which template file that this document should be injected into. Do not include a file extension when specifying a 
template name.

You can refer to the contents of a collection by the name of the folder that contains the collection files. For example, refer to the resources/collections directory
of this site. It contains a 'guides' collection with three files. The template file located at resources/pages/index.hbs iterates over all of these items, and in doing 
so gains access to each file's frontmatter. The template file takes advantage of that to generate a 'list' of guides, which is what you are seeing now.

As mentioned earlier, iterating over the contents of a collection gets you access to all frontmatter established at the top of each file, but Dagger also includes one 
other 'path' key that you can use to generate links to your content.