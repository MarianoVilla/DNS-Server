using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace codecrafters_dns_server.src
{
    public class DnsServer
    {
        public ILogger Logger { get; }
        public int Port { get; }
        public IPAddress Ip { get; }

        public DnsServer(ILogger Logger, int Port, IPAddress Ip)
        {
            this.Logger = Logger;
            this.Port = Port;
            this.Ip = Ip;
        }

        public void Start()
        {
            Logger.LogInformation($"Starting DNS server on {Ip}:{Port}");
            IPEndPoint Endpoint = new(Ip, Port);
            UdpClient Client = new(Endpoint);

            while (!ShouldStop)
            {
                IPEndPoint SourceEndpoint = new(IPAddress.Any, 0);
                byte[] Received = Client.Receive(ref SourceEndpoint);

                var Message = new DnsMessage(Received);

                byte[] Response = Message.GetResponse();

                Client.Send(Response, Response.Length, SourceEndpoint);
            }

        }
        public bool ShouldStop { get; set; }
        public void Stop()
        {
            ShouldStop = true;
        }
    }
}

