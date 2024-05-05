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

        public byte[] GetResponse()
        {
            var Response = new List<byte>();

            Header.ANCOUNT = 1;

            //Hardcoded stuff
            Header.QR = true;
            Header.AA = false;
            Header.TC = false;
            Header.RA = false;
            Header.Z = 0;
            Header.RCODE = (byte)(Header.OPCODE == 0 ? 0 : 4);

            var HeaderBytes = Header.GetBytes();
            HeaderBytes.Print(nameof(Header));
            Response.AddRange(HeaderBytes);

            var FirstQuestion = Questions.FirstOrDefault() 
                ?? throw new Exception("Expected at least one question, DUUUDE!");

            var FirstQuestionBytes = FirstQuestion.GetBytes();
            FirstQuestionBytes.Print(nameof(FirstQuestionBytes));
            Response.AddRange(FirstQuestionBytes);

            IEnumerable<byte> HardcodedAnswer =
                FirstQuestion.Name.GetBytes().Concat(new byte[] {
                0x00, 0x01, // Type
                0x00, 0x01, // Class
                0x00, 0x00, 0x00, 0x3c, //TTL
                0x00, 0x04, //RDLENGTH
                0x4c, 0x4c, 0x15, 0x15 //RDATA 
            });

            HardcodedAnswer.Print(nameof(HardcodedAnswer));

            Response.AddRange(HardcodedAnswer);

            return Response.ToArray();
        }
    }
}
