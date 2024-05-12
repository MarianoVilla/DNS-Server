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
        public IPEndPoint? ForwardServer { get; }

        public DnsServer(ILogger Logger, int Port, IPAddress Ip, IPEndPoint? ForwardServer = null)
        {
            this.Logger = Logger;
            this.Port = Port;
            this.Ip = Ip;
            this.ForwardServer = ForwardServer;
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

                Logger.LogBytes(RequestBytes, nameof(RequestBytes));

                var RequestMessage = new DnsMessage(RequestBytes, Logger);

                byte[] Response = ProcessRequest(RequestMessage).GetBytes();

                Client.Send(Response, Response.Length, SourceEndpoint);
            }

        }
        DnsMessage ForwardRequest(DnsMessage Request)
        {
            if(ForwardServer is null)
            {
                throw new InvalidOperationException("Attempted to forward request but no FWD server specified!");
            }
            UdpClient Client = new();
            Client.Connect(ForwardServer);
            Client.Send(Request.GetBytes());

            var RecvEndpoint = new IPEndPoint(IPAddress.Any, 0);
            var ResponseBytes = Client.Receive(ref RecvEndpoint);
            Logger.LogBytes(ResponseBytes, $"Response from FWD server");
            var ParsedResponse = new DnsMessage(ResponseBytes, Logger);
            return ParsedResponse;
        }
        DnsMessage ProcessRequest(DnsMessage Request)
        {
            Request.Header.ARCOUNT = 0;
            var Response = new DnsMessage(Logger)
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

                //byte[] HardcodedAnswerBytes =
                //    Question.Name.GetBytes().Concat(new byte[] {
                //0x00, 0x01, // Type
                //0x00, 0x01, // Class
                //0x00, 0x00, 0x00, 0x3c, //TTL
                //0x00, 0x04, //RDLENGTH
                //0x4c, 0x4c, 0x15, 0x15 //RDATA 
                //}).ToArray();

                var HardcodedAnswer = new DnsResourceRecord(Question.Name, 1, 1, 60, 
                    IPAddress.Parse("76.76.21.21"));

                DnsResourceRecord? Answer = null;
                if (ForwardServer is not null)
                {
                    Answer = ForwardRequest(Request).Answer.FirstOrDefault();
                }
                if (Answer is null)
                {
                    Answer = HardcodedAnswer;
                }

                Response.Questions.Add(Question);
                Response.Answer.Add(Answer);
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

