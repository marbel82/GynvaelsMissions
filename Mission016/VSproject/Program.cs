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
            ReadWAVEData();






            Console.WriteLine("");

            Console.ReadLine();
        }
        //_________________________________________________________________________________________
        static void ConvertFromBase64()
        {
            string comm = File.ReadAllText("mission016_comm.txt");

            byte[] commb =  Convert.FromBase64String(comm);

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
    }
}
