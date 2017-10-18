using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace Misja013
{
    class Program
    {
        static void Main(string[] args)
        {
            //CountAllColors();

            AnalyseStrangeRows();


            //LoadExtracted29();   // Convert bitmap to text

        }
        //_________________________________________________________________________________________
        public static void LoadExtracted29()
        {
            byte[] filedata = File.ReadAllBytes(@"_misja013.png.extracted\29");

            Dictionary<byte, int> his = new Dictionary<byte, int>();

            for (int b = 0; b < filedata.Length; b++)
            {
                if (!his.ContainsKey(filedata[b]))
                    his[filedata[b]] = 1;
                else
                    his[filedata[b]]++;

                if (filedata[b] == 255)
                    filedata[b] = 0;
                else if (filedata[b] == 1)
                    filedata[b] = 200;

            }
            File.WriteAllBytes("extracted29.data", filedata);

            StringBuilder sb = new StringBuilder();
            foreach (var h in his)
                sb.AppendLine($"{h.Key}\t{h.Value}");
            File.WriteAllText("extracted29.csv", sb.ToString());


        }
        //_________________________________________________________________________________________
        public static void AnalyseStrangeRows()
        {
            Bitmap bmp = new Bitmap("29data_in_grayscale.png");
            int W = bmp.Width;
            int H = bmp.Height;

            byte[] column = new byte[H];
            // Scan fifth column
            for (int y = 0; y < H; y++)
                column[y] = bmp.GetPixel(4, y).R;

             //column = column.Skip(2).ToArray(); // Shift array

            byte[] coltofilen = Array.ConvertAll(column, a => (a == 0) ? (byte)'0' : (byte)'1');
            File.WriteAllBytes("column5.txt", coltofilen);

            int groups8 = column.Length / 8;
            byte[] columnG8 = new byte[groups8];
            // Group each 8 bits
            for (int g = 0; g < groups8; g++)
                for (int b = 0; b < 8; b++)
                    if (column[g * 8 + b] == 0) // Inversion
                        columnG8[g] |= (byte)(1 << b);

            File.WriteAllBytes("column5_in_ascii.txt", columnG8);
        }
        //_________________________________________________________________________________________
        public static void CountAllColors()
        {
            Bitmap bmp = new Bitmap("misja013.png");
            int W = bmp.Width;
            int H = bmp.Height;

            Dictionary<Color, int> his = new Dictionary<Color, int>();

            for (int y = 0; y < H; y++)
                for (int x = 0; x < W; x++)
                {
                    Color p = bmp.GetPixel(x, y);
                    his[p] = (his.ContainsKey(p)) ? his[p] + 1 : 1;
                }

            StringBuilder sb = new StringBuilder();
            foreach (var kv in his)
                sb.AppendLine($"{kv.Key} = {kv.Value}");
            File.WriteAllText("all_colors.txt", sb.ToString());
        }
        //_________________________________________________________________________________________
        public static void LoadBitmapMisja013()
        {
            Bitmap bmp = new Bitmap("misja013.png");

            // Get image dimension
            int W = bmp.Width;
            int H = bmp.Height;

            Console.WriteLine($"Bitmap dimension: {W}x{H}");

            Dictionary<Color, int> his = new Dictionary<Color, int>();

            int[] hisInCol = Enumerable.Repeat(0, W).ToArray();
            int[] hisInRow = Enumerable.Repeat(0, H).ToArray();

            for (int y = 0; y < H; y++)
                for (int x = 0; x < W; x++)
                {
                    Color p = bmp.GetPixel(x, y);

                    if (!his.ContainsKey(p))
                        his[p] = 1;
                    else
                        his[p]++;

                    if (p.R == 0)
                    {
                        hisInCol[x]++;
                        hisInRow[y]++;
                    }
                }

            StringBuilder sb = new StringBuilder();
            foreach (var h in hisInCol)
                sb.AppendLine($"{h}");
            File.WriteAllText("cols_to_ascii.csv", sb.ToString());
            sb = new StringBuilder();
            foreach (var h in hisInRow)
                sb.AppendLine($"{h}");
            File.WriteAllText("rows_to_ascii.csv", sb.ToString());

            File.WriteAllBytes("cols_to_ascii.data", Array.ConvertAll(hisInCol, i => (byte)i));
            File.WriteAllBytes("rows_to_ascii.data", Array.ConvertAll(hisInRow, i => (byte)i));




            //// How many different colors were found
            //var dif_colors = his.SelectMany(x => x.Keys).Distinct();

            //Console.WriteLine("Colors used in bitmap:");
            //foreach (Color col in dif_colors)
            //    Console.WriteLine($"  {col}");

            //SaveToSCV("counts_in_rows.csv", his, dif_colors, (d, k) => d.ContainsKey(k) ? d[k] : -9);
            //SaveToSCV("counts_in_rows.csv", his, dif_colors, (d, k) => d.ContainsKey(k) ? d[k] : -9);

            //// Red color
            //Color colr = Color.FromArgb(255, 255, 0, 0);

            //StringBuilder sb = new StringBuilder();
            //foreach (var h in his)
            //{
            //    sb.Append((char)h[colr]);
            //}
            //// Save to file
            //File.WriteAllText("cols_to_ascii.txt", sb.ToString());
        }
        //_________________________________________________________________________________________
        public static void SaveToDataFile(string filename, Array arr)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var a in arr)
                sb.Append((char)a);
            File.WriteAllText(filename, sb.ToString());
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
