using System;
using System.IO;
using System.Threading.Tasks;

namespace ALSDecompress
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Invalid amount of arguments given, expected 2: input .als file, output directory for .xml.");
                return;
            }
            Console.WriteLine("Given path: " + args[0]);
            var input = args[0];
            var output = input.Remove(input.LastIndexOf("."));
            output += ".xml";
            var alsIoHandler = new ALSIOHandler(input, output);
            alsIoHandler.Decompress();
            alsIoHandler.StoreXmlData();
            alsIoHandler.WriteToXml();
        }
    }
}