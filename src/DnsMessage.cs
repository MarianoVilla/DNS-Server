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
        public List<DnsResourceRecord> Answer { get; set; }
        public DnsMessage(byte[] MessageBytes)
        {
            this.MessageBytes = MessageBytes ?? throw new ArgumentNullException(nameof(MessageBytes));
            this.Header = new DnsHeader(MessageBytes);

            ParseQuestions();
            ParseAnswers();
        }

        void ParseAnswers()
        {
            if (this.Header.ANCOUNT == 0)
                return;
            this.Answer = new List<DnsResourceRecord>();
            var RemainingBytes = MessageBytes[(12 + Questions.Sum(q => q.ByteCount))..];
            var AnswerRecordCount = this.Header.ANCOUNT;
            var ProcessedBytes = 0;
            while(AnswerRecordCount > 0)
            {
                AnswerRecordCount--;
                var RR = new DnsResourceRecord(RemainingBytes);
                this.Answer.Add(RR);
                ProcessedBytes += RR.ByteCount;
                RemainingBytes = RemainingBytes[ProcessedBytes..];
            }
        }
        void ParseQuestions()
        {
            var HeaderlessBytes = MessageBytes[12..];
            var QuestionCount = this.Header.QDCOUNT;
            var ProcessedBytes = 0;
            while (QuestionCount > 0)
            {
                QuestionCount--;
                var Q = new DnsQuestion(HeaderlessBytes);
                Questions.Add(Q);
                ProcessedBytes += Q.ByteCount;
                HeaderlessBytes = MessageBytes[ProcessedBytes..]; //WAIITTTTT, shouldn't this be HeaderlessBytes[..]?
            }
        }

        public byte[] GetResponse()
        {
            var Response = new List<byte>();

            Header.ANCOUNT = 1;

            Response.AddRange(Header.GetBytes());

            Response.AddRange(Questions.SelectMany(q => q.GetBytes()));

            byte[] AnswerBytes = new byte[] {
                0x0c, 0x63, 0x6f, 0x64, 0x65, 0x63, 0x72, 0x61, 0x66, 0x74, 0x65, 0x72,
                0x73, 0x02, 0x69, 0x6f, 0x00, //Name
                0x00, 0x01, // Type
                0x00, 0x01, // Class
                0x00, 0x00, 0x00, 0x3c, //TTL
                0x00, 0x04, //RDLENGTH
                0x4c, 0x4c, 0x15, 0x15 //RDATA
            };

            Response.AddRange(AnswerBytes);

            return Response.ToArray();
        }
    }
}
