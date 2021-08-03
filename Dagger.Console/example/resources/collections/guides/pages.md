---
template: detail
title: Pages
description: Creating new pages and utilizing your collections.
date: 2021-08-01
---
A page is a document written in plain HTML and is most often used to host other more dynamic content, such as posts in the context of a blog. 
For example, the [index](/) of this site is considered a page and is generated from the document located at <span class="hl">project/resources/pages/index.hbs</span>.

When Dagger examines a file that it finds within <span class="hl">project/resources/pages</span> it will eventually place the processed file in a directory similar to where it was found, 
with just a couple of small changes. The name of the file will become the name of a new directory within <span class="hl">project/site</span> and the file itself will now be titled index.html
and placed within that new directory.

A document at <span class="hl">project/resources/pages/about.hbs</span> will end up at <span class="hl">project/site/about/index.html</span>, meaning that you can reach this page by navigating to
domain.com/about when the site directory of your project is being hosted by a web server -- you get a clean URL without sacrificing a simple project layout during development.