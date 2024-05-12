using codecrafters_dns_server.src;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;

//byte[] byteArray = new byte[]
//{
//    0x0D, 0x5F, 0x01, 0x00, 0x00, 0x02, 0x00, 0x00,
//    0x00, 0x00, 0x00, 0x00, 0x03, 0x61, 0x62, 0x63,
//    0x11, 0x6C, 0x6F, 0x6E, 0x67, 0x61, 0x73, 0x73,
//    0x64, 0x6F, 0x6D, 0x61, 0x69, 0x6E, 0x6E, 0x61,
//    0x6D, 0x65, 0x03, 0x63, 0x6F, 0x6D, 0x00, 0x00,
//    0x01, 0x00, 0x01, 0x03, 0x64, 0x65, 0x66, 0xC0,
//    0x10, 0x00, 0x01, 0x00, 0x01
//};

//var M = new DnsMessage(byteArray);

//M.GetResponse().Print();
class Program
{
    static void Main(string[] args)
    {
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        ILogger Logger = factory.CreateLogger("Program");

        IPEndPoint? ForwardServer = null;
        if (args is not null && args.Length == 2 && args[0] == "--resolver")
        {
            var Splitted = args[1].Split(':');
            if(Splitted.Length != 2 ) 
            {
                var Error = $"Invalid resolver, expected ip:port, got {args[1]}";
                Logger.LogError(Error);
            }
            ForwardServer = new IPEndPoint(IPAddress.Parse(Splitted[0]), ushort.Parse(Splitted[1]));
        }


        var Server = new DnsServer(Logger, 2053, IPAddress.Parse("127.0.0.1"), ForwardServer);

        Server.Start();

    }
}
