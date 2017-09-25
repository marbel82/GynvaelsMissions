# Gynvael’s Mission 015


```
MISSION 015               goo.gl/JKN1Zq             DIFFICULTY: █████░░░░░ [5╱10]
┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅┅

One of our operatives managed to find an information leak vulnerability on an
internal website of a hostile syndicate. The vulnerability itself is quite
amusing, as it allows to leak any file on the FS in form of a bar chart. Our
operative leaked a script responsible for authenticating a system operator.

Your task is to analyze the chart and then the script, and attempt to recover
system operator's password.

  Chart: goo.gl/YbYYcS

Good luck!

---------------------------------------------------------------------------------

If you find the answer, put it in the comments under this video! If you write a
blogpost / post your solution / code online, please add a link as well!

P.S. I'll show/explain the solution on the stream in ~one week.
```

Downloaded chart:

![chart](https://github.com/marbel82/GynvaelsMissions/blob/master/Mission015/mission_15_leak%20shadow.png)

For each column I created a dictionary where the key is color, and the value is the number of occurrences.

``` C#
    Dictionary<Color, int>[] his = new Dictionary<Color, int>[W];

    Console.WriteLine($"Analyse rows...");
    for (int x = 0; x < W; x++)
    {
        his[x] = new Dictionary<Color, int>();
        for (int y = 0; y < H; y++)
        {
            Color p = bmp.GetPixel(x, y);

            if (!his[x].ContainsKey(p))
                his[x][p] = 1;
            else
                his[x][p]++;
        }
    }
```

And I saved the red color values to the file.

``` C#
    // Red color
    Color colr = Color.FromArgb(255, 255, 0, 0);

    StringBuilder sb = new StringBuilder();
    foreach (var h in his)
    {
        sb.Append((char)h[colr]);
    }
    
    // Save to file
    File.WriteAllText("cols_to_ascii.txt", sb.ToString());
```

Now we have php code:

``` php
<?php

if (!isset($_GET['password']) || !is_string($_GET['password'])) {
  die("bad password");
}

$p = $_GET['password'];

if (strlen($p) !== 25) {
  die("bad password");
}

if (md5($p) !== 'e66c97b8837d0328f3e5522ebb058f85') {
  die("bad password");
}

// Split the password in five and check the pieces.
// We need to be sure!
$values = array(
  0 => 'e6d9fe6df8fd2a07ca6636729d4a615a',
  5 => '273e97dc41693b152c71715d099a1049',
  10 => 'bd014fafb6f235929c73a6e9d5f1e458',
  15 => 'ab892a96d92d434432d23429483c0a39',
  20 => 'b56a807858d5948a4e4604c117a62c2d'
);

for ($i = 0; $i < 25; $i += 5) {
  if (md5(substr($p, $i, 5)) !== $values[$i]) {
    die("bad password");
  }
}

die("GW!");
```
