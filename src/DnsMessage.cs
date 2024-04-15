using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafters_dns_server.src
{
    public class DnsMessage
    {
        public byte[] MessageBytes { get; }
        public DnsHeader Header { get; set; }
        public List<DnsQuestion> Questions { get; set; } = new List<DnsQuestion>();
        public DnsMessage(byte[] MessageBytes)
        {
            this.MessageBytes = MessageBytes ?? throw new ArgumentNullException(nameof(MessageBytes));
            this.Header = new DnsHeader(MessageBytes);

            var HeaderlessBytes = MessageBytes[12..];
            var QuestionCount = this.Header.QDCOUNT;
            var ProcessedBytes = 0;
            while (QuestionCount > 0)
            {
                QuestionCount--;
                var Q = new DnsQuestion(HeaderlessBytes);
                Questions.Add(Q);
                ProcessedBytes += Q.ByteCount;
                HeaderlessBytes = MessageBytes[ProcessedBytes..];
            }
        }

        public byte[] GetResponse()
        {
            var Response = new byte[512];

            Header.GetBytes().CopyTo(Response, 0);

            int i = 12;
            foreach(DnsQuestion Q in Questions)
            {
                Q.GetBytes().CopyTo(Response, i);
                i += Q.ByteCount;
            }

            return Response;
        }
    }
}
