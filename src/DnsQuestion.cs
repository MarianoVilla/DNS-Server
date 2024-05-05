using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafters_dns_server.src
{
    public class DnsQuestion
    {
        public DnsName Name { get; set; }
        public ushort Type { get; set; }
        public ushort Class { get; set; }
        public int ByteCount => Name.ByteCount + 4;
        public DnsQuestion(byte[] QuestionsBytes, int Offset)
        {
            this.Name = new DnsName(QuestionsBytes, Offset);

            var NameBytes = Name.ByteCount + Offset;

            this.Type = (ushort)(QuestionsBytes[NameBytes] << 8 | QuestionsBytes[NameBytes + 1]);
            this.Class = (ushort)(QuestionsBytes[NameBytes + 2] << 8 | QuestionsBytes[NameBytes + 3]);
        }
        public byte[] GetBytes()
        { 
            var Bytes = new byte[this.ByteCount];
            var NameBytes = Name.GetBytes();

            Array.Copy(NameBytes, 0, Bytes, 0, NameBytes.Length);

            Bytes[NameBytes.Length] = (byte)(Type >> 8);
            Bytes[NameBytes.Length + 1] = (byte)(Type);
            Bytes[NameBytes.Length + 2] = (byte)(Class >> 8);
            Bytes[NameBytes.Length + 3] = (byte)(Class);

            return Bytes;
        }
    }
}
