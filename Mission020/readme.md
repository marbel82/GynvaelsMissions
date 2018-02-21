# Gynvael’s Mission 020


```
MISSION 020            goo.gl/rNHu7b               DIFFICULTY: ████░░░░░░ [4/10]
┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅
Dear Agent,

Another agent working on a case stumbled on this:

  goo.gl/mJjdrM

Looks pretty simple. See where it leads.

Good luck!
--
If you find the answer, put it in the comments under this video! If you write a
blogpost / post your solution / code online, please add a link as well!
If you tweet about it, include @gynvael to let me know :)

```
[Back to live stream](https://youtu.be/PiBfI7wltM8?t=6640)

# Solution (Python)

The first link leads us to:

![chart](https://github.com/marbel82/GynvaelsMissions/blob/master/Mission020/start.png)

After decoding on the phone, we get the text:

```
Calc value, add .png, repeat: 84905785,*577,-745,-342,*954,-672,+909,+644,-556,-524,*622
```


And when we calculate it, we get the name of the next file:

http://gynvael.coldwind.pl/qrmaze/29070456023771126.png

And so on. We must write a program.

``` python

eval(1+2)

```


The thousandth file contains a flag:

### Flag: QRCalcIsEasy﻿

![chart](https://github.com/marbel82/GynvaelsMissions/blob/master/Mission020/69129246053.png)

