current attempts to add pinyin are successful
but I cannot add zero-width-spaces to the chinese.
This is because baldur's gate 2 does not have
an understanding of zws, and thus displays the □ utf-8
default box.

This means that although adding pinyin itself
is not so hard, adding mouse-over word-lookups is.
As in, its not just... add them, then do some
GUI manipulation.

We have options -

1. we can modify the engine to understand zws but not
print a character for them

2. we can do in-game nlp (urgh)

3. we can just release the pinyin version