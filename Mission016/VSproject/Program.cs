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
            // Step 1
             ConvertFromBase64();

            // Step 2
            //ReadWAVEData();

            readWav("frombase64.wav", out float[] L, out float[] R);


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
                distsum2.Add((dist[i * 2] + dist[i * 2 + 1])/ 2);
            }
            dist = distsum2;

            StringBuilder sb = new StringBuilder();

            foreach (var d in dist)
            {
                sb.AppendLine($"{d}");
            }
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

            Console.WriteLine("");

            //Console.ReadLine();
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
