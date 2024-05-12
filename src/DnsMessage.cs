using Microsoft.Extensions.Logging;
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
        public ILogger Logger { get; }
        public DnsHeader Header { get; set; }
        public List<DnsQuestion> Questions { get; set; } = new List<DnsQuestion>();
        public List<DnsResourceRecord> Answer { get; set; } = new List<DnsResourceRecord>();
        public DnsMessage(byte[] MessageBytes, ILogger Logger)
        {
            this.MessageBytes = MessageBytes ?? throw new ArgumentNullException(nameof(MessageBytes));
            this.Logger = Logger;
            this.Header = new DnsHeader(MessageBytes);

            ParseQuestions();
            ParseAnswers();
        }
        public DnsMessage(ILogger Logger)
        {
            this.Logger = Logger;
        }

        void ParseAnswers()
        {
            if (this.Header.ANCOUNT == 0)
                return;
            var AnswerRecordCount = this.Header.ANCOUNT;
            var ProcessedBytes = 12 + Questions.Sum(q => q.ByteCount);
            while (AnswerRecordCount > 0)
            {
                AnswerRecordCount--;
                var RR = new DnsResourceRecord(MessageBytes, ProcessedBytes);
                this.Answer.Add(RR);
                ProcessedBytes += RR.ByteCount;
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

            var HeaderBytes = Header.GetBytes();
            Logger.LogBytes(HeaderBytes, nameof(HeaderBytes));
            Bytes.AddRange(HeaderBytes);

            foreach(var Q in Questions)
            {
                var QBytes = Q.GetBytes();
                Logger.LogBytes(QBytes, $"{nameof(QBytes)}");
                Bytes.AddRange(QBytes);

            }   

            foreach(var A in Answer)
            {
                var ABytes = A.GetBytes();
                Logger.LogBytes(ABytes, $"{nameof(ABytes)}");
                Bytes.AddRange(A.GetBytes());
            }

            return Bytes.ToArray();
        }
    }
}
