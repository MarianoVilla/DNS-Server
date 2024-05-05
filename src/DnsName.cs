using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafters_dns_server.src
{
    public class DnsName
    {
        public List<string> Labels { get; set; } = new List<string>();
        public int ByteCount { get; private set; }
        public DnsName(byte[] Bytes, int Offset)
        {
            ParseLabels(Bytes, Offset);
        }
        void ParseLabels(byte[] Bytes, int Offset)
        {
            int Pointer = Offset;
            bool Compressed = false;
            while (Bytes[Pointer] != 0)
            {
                if ((Bytes[Pointer] & 0b_1100_0000) == 192) //Is compressed
                {
                    Pointer = ((Bytes[Pointer] & 0b_0011_1111) << 8) | Bytes[Pointer + 1];
                    ByteCount += 2;
                    Compressed = true;
                }
                else
                {
                    var Length = Bytes[Pointer];
                    Pointer++;
                    Labels.Add(Encoding.ASCII.GetString(Bytes, Pointer, Length));
                    if(!Compressed)
                    {
                        ByteCount += Length + 1;
                    }
                    Pointer += Length;
                }
            }
            if (!Compressed)
            {
                ByteCount++;
            }
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
