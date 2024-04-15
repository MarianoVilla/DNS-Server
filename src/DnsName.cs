using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafters_dns_server.src
{
    public class DnsName
    {
        public List<string> Labels { get; set; } = new List<string>();
        public int ByteCount { get; private set; }
        public DnsName(byte[] Bytes)
        {
            while (ByteCount < Bytes.Length && Bytes[ByteCount] != 0)
            {
                var Length = Bytes[ByteCount];
                ByteCount++;
                Labels.Add(Encoding.ASCII.GetString(Bytes, ByteCount, Length));
                ByteCount += Length;
            }
            ByteCount++;
        }
        public byte[] GetBytes()
        {
            var Bytes = new List<byte>();
            foreach(var Label in Labels)
            {
                Bytes.Add((byte)Label.Length);
                Bytes.AddRange(Encoding.ASCII.GetBytes(Label));
            }
            Bytes.Add(0x00); //Hasta la vista, puto el que lee
            return Bytes.ToArray();
        }
    }
}
