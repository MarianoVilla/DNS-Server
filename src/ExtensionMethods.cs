using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafters_dns_server.src
{
    public static class ExtensionMethods
    {
        public static int ToInt(this bool Boolean)
        {
            return Boolean ? 1 : 0;
        }
        public static void Print(this IEnumerable<byte> Bytes, string? Title = null) 
        {
            Console.WriteLine($"************{Title}************");
            for (int i = 0; i < Bytes.Count(); i++)
            {
                Console.Write($"[{i}]: 0x{Bytes.ElementAt(i):X2}, ");
            }
            Console.WriteLine("\n");
        }

    }
}
