using System;
using System.Collections.Generic;
using System.Text;

using ChunkDecoder;

namespace Test
{
    class Program
    { 
        static void Main(string[] args)
        {
            ChunkDecoder.Decoder decoder = new ChunkDecoder.Decoder();
            decoder.ConsoleDebug = true;
            decoder.ProcessSignature = ProcessSignature;
            decoder.ProcessChunk = ProcessChunk;

            /*
            string val =
                "6\r\n" +
                "Hello \r\n" +
                "5\r\n" +
                "World\r\n" +
                "0\r\n\r\n";
            */

            string val =
                "6;chunk-signature=foobar;joelio;anotherone\r\n" +
                "Hello \r\n" +
                "5;chunk-signature=whatsup;watson;yetanotherone=hello\r\n" +
                "World\r\n" +
                "0\r\n\r\n";

            byte[] data = Encoding.UTF8.GetBytes(val);
            byte[] ret = null;
            if (decoder.Decode(data, out ret))
            {
                Console.WriteLine(Encoding.UTF8.GetString(ret));
            }
            else
            {
                Console.WriteLine("Failed");
            }

            Console.WriteLine("Press ENTER to exit");
            Console.ReadLine();
        }

        static bool ProcessSignature(KeyValuePair<string, string> kvp, byte[] data)
        {
            Console.WriteLine("Evaluating key-value pair: " + kvp.Key + "=" + kvp.Value);
            return true;
        }

        static bool ProcessChunk(byte[] data)
        {
            Console.WriteLine("Processing chunk: " + Encoding.UTF8.GetString(data));
            return true;
        }
    }
}
