using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.IO.Compression;

namespace Misja013
{
    class Program
    {
        static void Main(string[] args)
        {
            //CountAllColors();

            //AnalyseFifthColumn();

            SimpleSolution013();
        }
        //_________________________________________________________________________________________
        public static void SimpleSolution013()
        {
            //byte[] edata = File.ReadAllBytes(@"_misja013.png.extracted\29");
            byte[] edata = SimpleSolutionUnpack();

            byte[] txt = new byte[800];

            for (int i = 0, b = 0; i < edata.Length; i += 2401, b++)
                txt[b >> 3] |= (byte)(edata[i] << (b & 0x7));

            File.WriteAllBytes("solution.txt", txt);
        }
        //_________________________________________________________________________________________
        public static byte[] SimpleSolutionUnpack()
        {
            FileStream fs = new FileStream("misja013.png", FileMode.Open);
            fs.Seek(0x29+2, SeekOrigin.Begin);
            MemoryStream decomp = new MemoryStream();
            DeflateStream ds = new DeflateStream(fs, CompressionMode.Decompress);
            ds.CopyTo(decomp);
            return decomp.ToArray();
        }
        //_________________________________________________________________________________________
        /// <summary>
        /// Parses pixels in the fifth column in an image obtained from GIMP.
        /// </summary>
        public static void AnalyseFifthColumn()
        {
            Bitmap bmp = new Bitmap("29data_in_grayscale.png");

            byte[] column = new byte[bmp.Height];

            // Scan fifth column
            for (int y = 0; y < bmp.Height; y++)
                column[y] = bmp.GetPixel(4, y).R;

            //column = column.Skip(2).ToArray(); // Shift array

            // Save bits to text file
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
        /// <summary>
        /// Find all the colors in the picture.
        /// </summary>
        public static void CountAllColors()
        {
            Bitmap bmp = new Bitmap("misja013.png");

            Dictionary<Color, int> his = new Dictionary<Color, int>();

            for (int y = 0; y < bmp.Height; y++)
                for (int x = 0; x < bmp.Width; x++)
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

            int[] hisInCol = Enumerable.Repeat(0, W).ToArray();
            int[] hisInRow = Enumerable.Repeat(0, H).ToArray();

            // Count all the black pixels in the rows and columns
            for (int y = 0; y < H; y++)
                for (int x = 0; x < W; x++)
                {
                    Color p = bmp.GetPixel(x, y);

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
