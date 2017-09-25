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

    StringBuilder sbb = new StringBuilder();
    foreach (var h in his)
    {
        sbb.Append((char)h[colr]);
    }
    
    // Save to file
    File.WriteAllText("cols_to_ascii.txt", sbb.ToString());
```


