---
template: detail
title: Templates
description: An introduction to the templating system and a few words on future goals.
date: 2021-08-02
---
By default, Dagger projects are using a templating system known as [Handlebars](https://handlebarsjs.com/).
Handlebars is a templating language that was originally developed for use on Node.js and extends 
[Mustache](https://mustache.github.io/), a templating language written in Ruby.

If you aren't familiar with Handlebars, I recommend reading over the official
[documentation](https://handlebarsjs.com/guide/) first, and keep in mind that any tutorials on 
Handlebars you find should apply to your Dagger project templates. Dagger makes use of [Handlebars.Net](https://github.com/Handlebars-Net/Handlebars.Net) 
which provides a very similar API to the original Node.js library.

With a basic understanding of Handlebars, diving in and modifying the template files is the next step. 
The Handlebars documentation provides the large majority of necessary information, but you can also refer to
the Handlebars.Net documentation for examples specific to C# & .NET.

## Comments
Handlebars defines a [syntax](https://handlebarsjs.com/guide/#template-comments) for comments. The Handlebars 
documentation also goes on to state that HTML comments are not supported, however Handlebars.Net seems to accept HTML 
comments within Handlebars templates, so they are acceptable in a Dagger project.

## Inheritance
If you are familiar with Jinja2 from Python you might be looking for a block layouts feature, but Handlebars
supports no such thing. What it does support is the concept of [partials](https://handlebarsjs.com/guide/#partials). 
To see an example of partials being used in a Dagger project, refer to the document located
at <span class="hl">project/resources/pages/index.hbs</span>.

The <span class="hl">{{> header }}</span> and <span class="hl">{{> footer }}</span> tags at the top and bottom refer to partials that exist within the
<span class="hl">project/resources/templates/partials</span> directory. When building a site, Dagger is made aware of any existing partial files 
within this directory and makes them available for use in your other templates.

## Looping on Collections
Collections are made available for iteration by the name of the directory that they exist in, and looping over a collection
puts you in the scope of the files themselves, with access to all of the frontmatter and the transformed HTML body of the file
accessible through a <span class="hl">{{ body }}</span> tag. To see an example of this, refer to the document at <span class="hl">project/resources/templates/detail.hbs</span>,
which is a template file dedicated to displaying files from the guides collection in detail.

## Going Forward
Dagger is intended to ship with a variety of options for templating, including a templating engine designed specifically for Dagger,
but these features have no release date and Handlebars is currently the only available option for templating.