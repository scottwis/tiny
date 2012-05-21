DOC COMMENTS:

This project doesn't use XML doc comments, mainly because generating good text with them requires a lot of markup,
and entity escaping. Together they make anything other than trivial doc comments unreadable when reading the source code.

I'm not willing to sacrifice code readability for the ability to generate ugly looking web-pages no one will look at.

Instead, we use "markdown" to author comments. Mark down is readable as a normal comment, eliminates the need to escape
things like "<" and ">", and sucks WAAAAY less than XML.

In any case, a "markdown doc comment" can be specified using "//# " instead of "///".

Right now we don't have any processors for "markdown doc comments", which means we can't produce documents and the
comments won't show up in intellisence. It might be possible to write a Resharper or DXCore plugin that would parse the doc
comments and make them appear in intellisence. It's worth looking into at some point.

There are a few conventions we use:

Parameters are described by starting a line with "parameterName: " followed by the parameter documentation. For example:

//# x: The X coordinate for the circle's radius.
//# y :
//#     The y coordinate of the circle's radius.
//# radius: The radius of the circle.

Links to code elements are specified by placing the symbol name inside "[" and "]" simiar to how links are specified
in normal mark down. For example:

[System.Collectiosn.Generic.List<int>]
[System.Collectiosn.Generic.List<int>][list]
