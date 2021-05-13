using System;
using System.Linq;
using TOTOllyGeek.Awesome.Lib;

namespace TOTOllyGeek.Awesome
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("You need to specify some text!");
                return -1;
            }

            var figlets = args.Select(a => new FigMe(a).ToString());
            
            foreach (var figlet in figlets)
            {
                Console.WriteLine(figlet);
            }

            return 0;
        }
    }
}