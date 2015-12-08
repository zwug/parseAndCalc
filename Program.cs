using System;
using System.IO;

namespace Equatiator
{
    class Program
    {
        static void Main(string[] args)
        {
            Stream inputStream = Console.OpenStandardInput();
            Console.WriteLine("Write an expression starting with \"calc\" and press enter:");
            Scanner scanner = new Scanner(inputStream);
            Parser parser = new Parser(scanner);
            while(true)
            {
                parser.Parse();
            }
        }
    }
}
