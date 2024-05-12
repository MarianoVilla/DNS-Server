using Microsoft.Extensions.Logging;
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
        public static void LogBytes(this ILogger Logger, byte[] Bytes, 
            string? Title = null, LogLevel Level = LogLevel.Information) 
        {
            Logger.Log(Level, $"************{Title}************\n" +
                string.Join(", ", Bytes.Select((b, i) => $"[{i}]: 0x{b:X2}, ")));
        }

    }
}
