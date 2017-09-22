using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Mission015
{
    class Program
    {
        static void Main(string[] args)
        {

            LoadBitmapMission015();

            //GenerateRandomBitmap();

        }
        public static void LoadBitmapMission015()
        {
            Bitmap bmp = new Bitmap("mission_15_leak.png");

            // Get image dimension
            int W = bmp.Width;
            int H = bmp.Height;

            Console.WriteLine($"Bitmap dimension: {W}x{H}");

            Dictionary<Color, int>[] his = new Dictionary<Color, int>[W];
            for (int x = 0; x < his.Length; x++)
                his[x] = new Dictionary<Color, int>();

            Console.WriteLine($"Analyse rows...");
            for (int x = 0; x < W; x++)
            {
                for (int y = 0; y < H; y++)
                {
                    Color p = bmp.GetPixel(x, y);

                    if (!his[x].ContainsKey(p))
                        his[x][p] = 1;
                    else
                        his[x][p]++;  
                }
            }

            var dif_colors = his.SelectMany(x => x.Keys).Distinct();

            Console.WriteLine("Colors used in bitmap:");
            foreach (Color col in dif_colors)
                Console.WriteLine($"  {col}");

            //SaveToSCV("counts_in_cols.csv", his, dif_colors, (d, k) => d[k]);

            Color colr = Color.FromArgb(255, 255, 0, 0);

            StringBuilder sbb = new StringBuilder();
            foreach (var h in his)
            {
                sbb.Append((char)h[colr]);
            }
            // Save to file
            File.WriteAllText("cols_to_ascii.txt", sbb.ToString());

            // Print colors in each column
            Console.WriteLine("Rows:");
            for (int x = 0; x < his.Length; x++)
            {
                string s = string.Format(" x {0,4}: ", x);
                foreach (KeyValuePair<Color, int> h in his[x])
                {
                    s += $"{h.Key}={h.Value} ";
                }
                Console.WriteLine(s);
            }
        }

        public static void SaveToSCV<K, V>(string filename, IEnumerable<IDictionary<K, V>> items,
            IEnumerable<K> keys, Func<IDictionary<K, V>, K, V> valueFun)
        {
            StringBuilder csv = new StringBuilder();
            // List all `his` elements, print each color count separated by a semicolumn
            foreach (var item in items)
            {
                bool sec = false;
                foreach (K key in keys)
                {
                    if (sec)
                        csv.Append(";");
                    else
                        sec = true;

                    csv.Append($"{valueFun(item, key)}");
                }
                csv.AppendLine();
            }
            // Save to CSV file
            File.WriteAllText(filename, csv.ToString());
        }
        /*
            StringBuilder csv = new StringBuilder();
            // List all `his` elements, print each color count separated by a semicolumn
            for (int x = 0; x < his.Length; x++)
            {
                bool sec = false;
                foreach (Color col in dif_colors) 
                {
                    if (sec)
                        csv.Append(";");
                    else
                        sec = true;

                    csv.Append($"{his[x][col]}"); 
                }
                csv.AppendLine();
            }
            // Save to CSV file
            File.WriteAllText("counts_in_cols.csv", csv.ToString());        */

        public static void GenerateRandomBitmap()
        {
            int width = 640, height = 320;

            //bitmap
            Bitmap bmp = new Bitmap(width, height);

            //random number
            Random rand = new Random();

            //create random pixels
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //generate random ARGB value
                    int a = rand.Next(256);
                    int r = rand.Next(256);
                    int g = rand.Next(256);
                    int b = rand.Next(256);

                    //set ARGB value
                    bmp.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                }
            }

            //save (write) random pixel image
            bmp.Save("RandomImage.png");
        }
    }
}
