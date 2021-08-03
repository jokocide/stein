---
template: detail
title: Templates
description: An introduction to the templating system and a few words on future goals.
date: 2021-08-02
---
By default, Dagger projects are using a templating system known as [Handlebars](https://handlebarsjs.com/).
Handlebars is a templating language that was originally developed for use on Node.js and extends 
[Mustache](https://mustache.github.io/), which was originally developed for Ruby.

If you aren't familiar with Handlebars, I recommend reading over the official
[documentation](https://handlebarsjs.com/guide/) first, and keep in mind any tutorials on 
Handlebars that you find will apply to your Dagger project templates. Dagger makes use of Handlebars.Net 
which provides a very similar API to the original Node.js library.

With a basic understanding of Handlebars, diving in and modifying the template files is the next step!

## Comments
Handlebars provides their own [syntax](https://handlebarsjs.com/guide/#template-comments) for comments. The Handlebars 
documentation also goes on to state that HTML comments are not supported, however Handlebars.Net seems to accept HTML 
comments within Handlebars templates, so this is your choice.

## Inheritance
If you are familiar with Jinja2 from Python you might be looking for a block layouts feature, but Handlebars
supports no such thing. What it does support is the concept of [partials](https://handlebarsjs.com/guide/#partials). 
To see an example of partials being used in a Dagger project, refer to the document located within the example site 
at 'project/resources/pages/index.hbs'.

The '{{> header }}' and '{{> footer }}' tags at the top and bottom refer to partials that exist within the
'project/resources/templates/partials' directory. When building a site, Dagger is made aware of any existing partial files 
and serves them up for use in your other templates.

It is recommended to make any reusable piece of code a partial that can be imported anywhere you need it and easily updated
later on.

## Looping on Collections
Collections are made available for iteration by the name of the directory that they exist in, and looping over a collection
puts you in the scope of the files themselves, with access to all of the frontmatter and the transformed HTML body of the file,
accessible through a {{ body }} tag. To see an example of this, refer to the document at 'project/resources/templates/detail.hbs'
which is a template file dedicated to displaying files from the guides collection in detail.

## Going Forward
Dagger is intended to ship with a variety of options for templating, including a templating engine designed specifically for Dagger,
but these features have no release date and Handlebars is currently the only available option for templating.