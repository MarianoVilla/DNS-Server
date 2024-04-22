using codecrafters_dns_server.src;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");


using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
ILogger Logger = factory.CreateLogger("Program");

var Server = new DnsServer(Logger, 2053, IPAddress.Parse("127.0.0.1"));

Server.Start();
