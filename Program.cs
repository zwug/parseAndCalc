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
            while (true)
            {
                parser.Parse();
            }
        }
        public int calculateExpression(int leftOp, int rightOp, string operation)
        {
            switch (operation) {
                case "PLUS":
                    return leftOp + rightOp;
                case "MINUS":
                    return leftOp - rightOp;
                case "MUL":
                    return leftOp * rightOp;
                case "DIV":
                    return leftOp / rightOp;
                default:
                    return 0;
            }
        }
    }
}
