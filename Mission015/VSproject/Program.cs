using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;

namespace Mission015
{
    class Program
    {
        //_________________________________________________________________________________________
        static void Main(string[] args)
        {
            // First Part
            //LoadBitmapMission015();   // Convert bitmap to text

            // Second Part
            if (args.Length < 1)
            {
                Console.WriteLine("Type: mission015 <md5>");
                Console.WriteLine("eg.: mission015 e6d9fe6df8fd2a07ca6636729d4a615a");
                return;
            }
            if (args[0].Length != 32)
            {
                Console.WriteLine($"Bad parameter: {args[0]}");
                return;
            }

            //string tmd5 = ComputeMD5("AAZAA");

            // Finding a matching hash
            string guesspass = Find5CharPassword(args[0]);

            Console.WriteLine("Found password:");
            Console.WriteLine((guesspass != null) ? $"\"{guesspass}\"" : " :( Not found");   

            Console.ReadLine();
        }
        //_________________________________________________________________________________________
        public static string ComputeMD5(string pass)
        {
            using (MD5 md5 = MD5.Create())
            {
                return BitConverter.ToString(md5.ComputeHash(Encoding.ASCII.GetBytes(pass))).Replace("-", null).ToLower();
            }
        }
        //_________________________________________________________________________________________
        // Finds MD5 for passMD5
        public static string Find5CharPassword(string passMD5)
        {
            long it = 0;
            byte[] passhash = StringToByteArray(passMD5);
            //byte[] currpass = new byte[5] { (byte)'A', (byte)'A', (byte)'A', (byte)'A', (byte)'A' };
            byte[] currpass = new byte[5] { 32, 32, 32, 32, 32 };

            Stopwatch watch = new Stopwatch();
            watch.Start();

            const long cwl_step = 1000000;

            using (MD5 md5 = MD5.Create())
            {
                do
                {
                    // Print some progress info
                    it++;
                    if ((it % cwl_step) == 0)
                    {
                        long md5ps = (long)((double)cwl_step * 1000 / watch.ElapsedMilliseconds);
                        watch.Restart();
                        Console.WriteLine($"({it}) {Encoding.ASCII.GetString(currpass)}  - {md5ps} md5/s");
                    }

                    byte[] ps_md5 = md5.ComputeHash(currpass);

                    if (passhash.SequenceEqual(ps_md5))
                    {
                        return Encoding.ASCII.GetString(currpass);
                    }

                } while (NextPass(currpass));

                return null;
            }
        }
        //_________________________________________________________________________________________
        // Return next permutation in table: {65,32,32}, {66,32,32},
        // ... {'z',66,32}, {32,67,32}   itd.
        // Subsequent bytes: ' ', '!', A-Z, a-z
        public static bool NextPass(byte[] ba)
        {
            int pos = 0;
            bool continu = true;
            do
            {
                if (ba[pos] == 'z')
                {
                    ba[pos] = 32;
                    pos++;
                    if (pos >= ba.Length)
                        return false;
                }
                else
                {
                    if (ba[pos] == 32)
                        ba[pos] = (byte)'!';
                    else if (ba[pos] == '!')
                        ba[pos] = (byte)'A';
                    else if (ba[pos] == 'Z')
                        ba[pos] = (byte)'a';
                    else
                        ba[pos]++;
                    continu = false;
                }
            } while (continu);
            return true;
        }
        //_________________________________________________________________________________________
        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
        //_________________________________________________________________________________________
        public static void LoadBitmapMission015()
        {
            Bitmap bmp = new Bitmap("mission_15_leak.png");

            // Get image dimension
            int W = bmp.Width;
            int H = bmp.Height;

            Console.WriteLine($"Bitmap dimension: {W}x{H}");

            Dictionary<Color, int>[] his = new Dictionary<Color, int>[W];

            Console.WriteLine("Analyse image...");
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

            // How many different colors were found
            var dif_colors = his.SelectMany(x => x.Keys).Distinct();

            Console.WriteLine("Colors used in bitmap:");
            foreach (Color col in dif_colors)
                Console.WriteLine($"  {col}");

            //SaveToSCV("counts_in_cols.csv", his, dif_colors, (d, k) => d[k]);

            // Red color
            Color colr = Color.FromArgb(255, 255, 0, 0);

            StringBuilder sb = new StringBuilder();
            foreach (var h in his)
            {
                sb.Append((char)h[colr]);
            }
            // Save to file
            File.WriteAllText("cols_to_ascii.txt", sb.ToString());
        }
        //_________________________________________________________________________________________
        // Save items to SVG file.
        // Each item is placed in a row:
        // row: valueFun(item, keys[first]);<valueFun(item, keys[second]);...
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
        //_________________________________________________________________________________________
    }
}
