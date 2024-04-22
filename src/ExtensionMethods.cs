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
    }
}
