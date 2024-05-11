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
        public List<DnsResourceRecord> Answer { get; set; } = new List<DnsResourceRecord>();
        public DnsMessage(byte[] MessageBytes)
        {
            this.MessageBytes = MessageBytes ?? throw new ArgumentNullException(nameof(MessageBytes));
            this.Header = new DnsHeader(MessageBytes);

            ParseQuestions();
            ParseAnswers();
        }
        public DnsMessage()
        {
            
        }

        void ParseAnswers()
        {
            if (this.Header.ANCOUNT == 0)
                return;
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
            var QuestionCount = this.Header.QDCOUNT;
            var ProcessedBytes = 12;
            while (QuestionCount > 0)
            {
                QuestionCount--;
                var Q = new DnsQuestion(MessageBytes, ProcessedBytes);
                Questions.Add(Q);
                ProcessedBytes += Q.ByteCount;
            }
        }

        public byte[] GetBytes()
        {
            var Bytes = new List<byte>();

            Bytes.AddRange(Header.GetBytes());

            foreach(var Q in Questions)
            {
                Bytes.AddRange(Q.GetBytes());
            }   

            foreach(var A in Answer)
            {
                Bytes.AddRange(A.GetBytes());
            }

            return Bytes.ToArray();
        }
    }
}
