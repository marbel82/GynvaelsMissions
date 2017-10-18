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
using MyCommon;

namespace Mission016
{
    class Program
    {
        //_________________________________________________________________________________________
        static void Main(string[] args)
        {
            // Step 1
            //ConvertFromBase64();

            // Step 2
            //--ReadWAVEData();    
            readWav("frombase64.wav", out float[] L, out float[] R);

            //--FindSomething1(L);    
            FindSomething2(L);

            // Step 3
            FindPlayfair();

            Console.WriteLine("");

            //Console.ReadLine();
        }
        //_________________________________________________________________________________________
        static void FindPlayfair()
        {
            char[,] T = new char[5, 5] {      // y, x
                { '#', '#', 'R', 'O', 'N'},
                { 'D', 'I', 'Y', 'M', 'A'},
                { 'U', 'Z', '#', '#', '#'},
                { 'B', 'C', 'K', 'P', '#'},
                { '#', '#', 'V', 'W', 'X'},
            };

            string pass0 = "Y DHXDMW BQLF KDYNV";
            string pass = "YDHXDMWBQLFKDYNV";
            char[] miss = new char[9] { 'E', 'F', 'G', 'H', 'J', 'L', 'Q', 'S', 'T' };


            StringBuilder sb = new StringBuilder();

            Permutation perm = new Permutation(8);

            while (perm.Next())
            {
                T = PutMissToTab(T, miss, perm);
                string decpass = Decrypt(T, pass);
                sb.AppendLine($"{decpass}");
            }

            File.WriteAllText("playfair.txt", sb.ToString());

        }
        static char[,] PutMissToTab(char[,] tab55, char[] miss, Permutation perm)
        {
            //char[,] T = (char[,])tab55.Clone();
            char[,] T = tab55;

            T[0, 0] = miss[perm.Perm[0]];
            T[0, 1] = miss[perm.Perm[1]];
            T[2, 2] = miss[perm.Perm[2]];
            T[2, 3] = miss[perm.Perm[3]];
            T[2, 4] = miss[perm.Perm[4]];
            T[3, 4] = miss[perm.Perm[5]];
            T[4, 0] = miss[perm.Perm[6]];
            T[4, 1] = miss[perm.Perm[7]];
            return T;
        }

        static string Decrypt(char[,] tab, string passc)
        {
            int len = passc.Length;
            string pa = "";

            for (int i = 0; i < len; i += 2)
            {
                char c1 = passc[i];
                char c2 = passc[i + 1];

                FindReversePair(tab, ref c1, ref c2);

                pa += c1;
                pa += c2;
            }              

            return pa.Insert(1, " ").Insert(8, " ").Insert(13, " ");
        }
        static void FindReversePair(char[,] tab, ref char c1, ref char c2)
        {
            int x1, x2, y1, y2;
            FindPos(tab, c1, out y1, out x1);
            FindPos(tab, c2, out y2, out x2);

            if (y1 == y2)
            {
                x1 = dec(x1);
                x2 = dec(x2);
            }
            else if (x1 == x2)
            {
                y1 = dec(y1);
                y2 = dec(y2);
            }
            else
            {
                int xx = x1;
                x1 = x2;
                x2 = xx;
            }

            c1 = tab[y1, x1];
            c2 = tab[y2, x2];     

        }
        static int dec(int p)
        {
            if (p == 0)
                return 4;
            else
                return p - 1;
        }
        static void FindPos(char[,] tab, char c, out int y, out int x)
        {
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    if (tab[i, j] == c)
                    {
                        x = j;
                        y = i;
                        return;
                    }
            throw new Exception("Something went wrong!");
        }
        //_________________________________________________________________________________________
        static void FindSomething2(float[] L)
        {
            List<double> int0 = new List<double>();
            List<double> freq = new List<double>();

            for (int t = 1; t < L.Length; t++)
            {
                if (((L[t - 1] <= 0) && (L[t] > 0))
                    || ((L[t - 1] >= 0) && (L[t] < 0)))
                {
                    // Calculation of linear function parameters
                    double t1 = t - 1;
                    //double t2 = t;
                    double y1 = L[t - 1];
                    double y2 = L[t];

                    //double a = (y2 - y1) / t2 - t1;
                    double a = (y2 - y1);    // t2 - t1 == 1
                    double b = y1 - a * t1;

                    // Intersection with OX axis
                    double t0 = -b / a;

                    int0.Add(t0);
                }

                if (int0.Count >= 2)
                {
                    if (t % 4 == 0)
                    {
                        double h = int0[int0.Count - 1] - int0[int0.Count - 2];
                        freq.Add(h);
                    }
                }
            }

            SaveToCSV("freq.csv", freq);
            //===================================
            double fmin = freq.Min();
            double fmax = freq.Max();
            byte[] freqb = new byte[freq.Count];
            for (int i = 0; i < freq.Count; i++)
            {
                //freqb[i] = (byte)((48000 / dist[i]) / 50);
                freqb[i] = (byte)((freq[i] - 10) * 20);
                //distb[i] = (byte)(dist[i] + 65 - dmin);
                //if (distb[i] == 'F') distb[i] = (byte)'.';
                //if (distb[i] == 'A') distb[i] = (byte)'#';
                //distb[i] = (byte)(dist[i] + '0'- mind);
            }
            File.WriteAllBytes("freqd.data", freqb);
            //===================================
            List<double> dif = new List<double>();

            for (int i = 1; i < int0.Count; i++)
            {
                double h = int0[i] - int0[i - 1];
                h = ((int)(h * 10)) / 10.0;
                //h = ((int)(h * 10));
                dif.Add(h);
            }
            //===================================
            //List<double> freq = new List<double>();
            //for (int t = 0; t < L.Length; t += 2)
            //{
            //    double tfind = t;
            //    var tt = int0.Select((tint, index) => new { tint, index }).FirstOrDefault(q => q.tint > tfind);


            //    freq.Add(dif[(tt.index - 2) < 0 ? 0 : tt.index - 2]);

            //}


            //===================================
            int dif2count = dif.Count / 2;
            List<double> difsum2 = new List<double>();

            for (int i = 0; i < dif2count; i++)
                difsum2.Add(dif[i * 2] + dif[i * 2 + 1]);
            dif = difsum2;



            //===================================

            List<double> dist = new List<double>();
            List<int> histo = new List<int>();

            double prev = dif[0];
            dist.Add(prev);
            histo.Add(1);
            for (int i = 1; i < dif.Count; i++)
            {
                if (dif[i] != prev)
                {
                    prev = dif[i];
                    dist.Add(prev);
                    histo.Add(1);
                }
                else
                    histo[histo.Count - 1]++;
            }
            //===================================

            SaveToCSV("dist.csv", dif);

            double dmin = dist.Min();
            byte[] distb = new byte[dist.Count];
            for (int i = 0; i < dist.Count; i++)
            {
                //distb[i] = (byte)((48000 / dist[i]) / 50);
                distb[i] = (byte)(dist[i] * 6);
                //distb[i] = (byte)(dist[i] + 65 - dmin);
                //if (distb[i] == 'F') distb[i] = (byte)'.';
                //if (distb[i] == 'A') distb[i] = (byte)'#';
                //distb[i] = (byte)(dist[i] + '0'- mind);
            }
            File.WriteAllBytes("distb.data", distb);

            int hmin = histo.Min();
            byte[] histob = new byte[histo.Count];
            for (int i = 0; i < histo.Count; i++)
            {
                histob[i] = (byte)(histo[i] + '0' - hmin);
                //if (distb[i] == 'F') distb[i] = (byte)'.';
                //if (distb[i] == 'A') distb[i] = (byte)'#';
                //distb[i] = (byte)(dist[i] + '0'- mind);
            }
            File.WriteAllBytes("histob.txt", histob);

            SaveToCSV("histo.txt", histo.Where(x => x > 1));
        }

        //_________________________________________________________________________________________
        static void SaveToCSV<T>(string filename, IEnumerable<T> col)
        {
            StringBuilder sb = new StringBuilder();
            //int0.Select(d => { sb.AppendLine($"{d}"); return 0; }).Count();
            foreach (var c in col) sb.AppendLine($"{c}");
            File.WriteAllText(filename, sb.ToString());
        }
        //_________________________________________________________________________________________
        static void FindSomething1(float[] L)
        {
            List<int> arr = new List<int>();

            for (int i = 1; i < L.Length; i++)
            {
                if ((L[i - 1] <= 0) && (L[i] > 0))
                    arr.Add(i);
                else if ((L[i - 1] >= 0) && (L[i] < 0))
                    arr.Add(i);
            }

            List<int> dif = new List<int>();

            for (int i = 1; i < arr.Count; i++)
            {
                //dif.Add((arr[i] - arr[i - 1]) / 2 * 2);
                dif.Add((arr[i] - arr[i - 1]) / 2);
                //dif.Add(arr[i] - arr[i - 1]);    
            }

            //int dif2count = dif.Count / 2;
            //List<int> difsum2 = new List<int>();

            //for (int i = 1; i < dif2count; i++)
            //{
            //    difsum2.Add(dif[i * 2] + dif[i * 2 + 1]);
            //}
            //dif = difsum2;

            List<int> dist = new List<int>();
            List<int> histo = new List<int>();

            int prev = dif[0];
            dist.Add(prev);
            histo.Add(1);
            for (int i = 1; i < dif.Count; i++)
            {
                if (dif[i] != prev)
                {
                    prev = dif[i];
                    dist.Add(prev);
                    histo.Add(1);
                }
                else
                    histo[histo.Count - 1]++;
            }

            int dist2count = dist.Count / 2;
            List<int> distsum2 = new List<int>();
            for (int i = 1; i < dist2count; i++)
            {
                distsum2.Add((dist[i * 2] + dist[i * 2 + 1]) / 2);
            }
            dist = distsum2;

            StringBuilder sb = new StringBuilder();
            foreach (var d in dist) sb.AppendLine($"{d}");
            File.WriteAllText("dist.csv", sb.ToString());

            int dmin = dist.Min();

            byte[] distb = new byte[dist.Count];
            for (int i = 0; i < dist.Count; i++)
            {
                distb[i] = (byte)(dist[i] + 65 - dmin);
                //if (distb[i] == 'F') distb[i] = (byte)'.';
                //if (distb[i] == 'A') distb[i] = (byte)'#';
                //distb[i] = (byte)(dist[i] + '0'- mind);
            }
            File.WriteAllBytes("distb.txt", distb);

            int hmin = histo.Min();
            byte[] histob = new byte[histo.Count];
            for (int i = 0; i < histo.Count; i++)
            {
                histob[i] = (byte)(histo[i] + '0' - hmin);
                //if (distb[i] == 'F') distb[i] = (byte)'.';
                //if (distb[i] == 'A') distb[i] = (byte)'#';
                //distb[i] = (byte)(dist[i] + '0'- mind);
            }
            File.WriteAllBytes("histob.txt", histob);
        }

        //_________________________________________________________________________________________
        static void ConvertFromBase64()
        {
            string comm = File.ReadAllText("mission016_comm.txt");

            byte[] commb = Convert.FromBase64String(comm);

            File.WriteAllBytes("frombase64.txt", commb);

        }
        //_________________________________________________________________________________________
        static void ReadWAVEData()
        {
            FileStream fs = new FileStream("frombase64.wav", FileMode.Open);

            BinaryReader reader = new BinaryReader(fs);

            int chunkID = reader.ReadInt32();
            int fileSize = reader.ReadInt32();
            int riffType = reader.ReadInt32();
            int fmtID = reader.ReadInt32();
            int fmtSize = reader.ReadInt32();
            int fmtCode = reader.ReadInt16();
            int channels = reader.ReadInt16();
            int sampleRate = reader.ReadInt32();
            int fmtAvgBPS = reader.ReadInt32();
            int fmtBlockAlign = reader.ReadInt16();
            int bitDepth = reader.ReadInt16();

            if (fmtSize == 18)
            {
                // Read any extra values
                int fmtExtraSize = reader.ReadInt16();
                reader.ReadBytes(fmtExtraSize);
            }

            int dataID = reader.ReadInt32();
            int dataSize = reader.ReadInt32();

            byte[] byteArray = reader.ReadBytes(dataSize);

        }

        //_________________________________________________________________________________________
        static bool readWav(string filename, out float[] L, out float[] R)
        {
            L = R = null;
            //float [] left = new float[1];

            //float [] right;
            try
            {
                using (FileStream fs = File.Open(filename, FileMode.Open))
                {
                    BinaryReader reader = new BinaryReader(fs);

                    // chunk 0
                    int chunkID = reader.ReadInt32();
                    int fileSize = reader.ReadInt32();
                    int riffType = reader.ReadInt32();


                    // chunk 1
                    int fmtID = reader.ReadInt32();
                    int fmtSize = reader.ReadInt32(); // bytes for this chunk
                    int fmtCode = reader.ReadInt16();
                    int channels = reader.ReadInt16();
                    int sampleRate = reader.ReadInt32();
                    int byteRate = reader.ReadInt32();
                    int fmtBlockAlign = reader.ReadInt16();
                    int bitDepth = reader.ReadInt16();

                    if (fmtSize == 18)
                    {
                        // Read any extra values
                        int fmtExtraSize = reader.ReadInt16();
                        reader.ReadBytes(fmtExtraSize);
                    }

                    // chunk 2
                    int dataID = reader.ReadInt32();
                    int bytes = reader.ReadInt32();

                    // DATA!
                    byte[] byteArray = reader.ReadBytes(bytes);

                    int bytesForSamp = bitDepth / 8;
                    int samps = bytes / bytesForSamp;


                    float[] asFloat = null;
                    switch (bitDepth)
                    {
                        case 64:
                            double[]
                            asDouble = new double[samps];
                            Buffer.BlockCopy(byteArray, 0, asDouble, 0, bytes);
                            asFloat = Array.ConvertAll(asDouble, e => (float)e);
                            break;
                        case 32:
                            asFloat = new float[samps];
                            Buffer.BlockCopy(byteArray, 0, asFloat, 0, bytes);
                            break;
                        case 16:
                            Int16[]
                            asInt16 = new Int16[samps];
                            Buffer.BlockCopy(byteArray, 0, asInt16, 0, bytes);
                            asFloat = Array.ConvertAll(asInt16, e => e / (float)Int16.MaxValue);
                            break;
                        default:
                            return false;
                    }

                    switch (channels)
                    {
                        case 1:
                            L = asFloat;
                            R = null;
                            return true;
                        case 2:
                            L = new float[samps];
                            R = new float[samps];
                            for (int i = 0, s = 0; i < samps; i++)
                            {
                                L[i] = asFloat[s++];
                                R[i] = asFloat[s++];
                            }
                            return true;
                        default:
                            return false;
                    }
                }
            }
            catch
            {
                Console.WriteLine("...Failed to load note: " + filename);
                return false;
                //left = new float[ 1 ]{ 0f };
            }

            return false;
        }
        //_________________________________________________________________________________________

    }
}
