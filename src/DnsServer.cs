using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.PortableExecutable;

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
                byte[] RequestBytes = Client.Receive(ref SourceEndpoint);

                RequestBytes.Print(nameof(RequestBytes));

                var RequestMessage = new DnsMessage(RequestBytes);

                byte[] Response = ProcessRequest(RequestMessage).GetBytes();

                Client.Send(Response, Response.Length, SourceEndpoint);
            }

        }
        DnsMessage ProcessRequest(DnsMessage Request)
        {
            var Response = new DnsMessage()
            {
                Header = new DnsHeader(Request.Header.GetBytes())
            };

            //Hardcoded stuff
            Response.Header.QR = true;
            Response.Header.AA = false;
            Response.Header.TC = false;
            Response.Header.RA = false;
            Response.Header.Z = 0;
            Response.Header.RCODE = (byte)(Response.Header.OPCODE == 0 ? 0 : 4);
            Response.Header.ARCOUNT = 0;

            foreach (var Question in Request.Questions)
            {

                byte[] HardcodedAnswerBytes =
                    Question.Name.GetBytes().Concat(new byte[] {
                0x00, 0x01, // Type
                0x00, 0x01, // Class
                0x00, 0x00, 0x00, 0x3c, //TTL
                0x00, 0x04, //RDLENGTH
                0x4c, 0x4c, 0x15, 0x15 //RDATA 
                }).ToArray();

                var HardcodedAnswer = new DnsResourceRecord(HardcodedAnswerBytes);

                Response.Questions.Add(Question);
                Response.Answer.Add(HardcodedAnswer);
                Response.Header.ANCOUNT++;
            }

            return Response;
        }
        public bool ShouldStop { get; private set; }
        public void Stop()
        {
            ShouldStop = true;
        }
    }
}

