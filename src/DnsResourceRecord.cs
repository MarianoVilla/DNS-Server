using System.Net;

namespace codecrafters_dns_server.src
{
    public class DnsResourceRecord
    {
        public int ByteCount { get; set; }
        public DnsName Name { get; set; }
        public ushort Type { get; set; }
        public ushort Class { get; set; }
        public int TTL { get; set; }
        public ushort RDLENGTH { get; set; }
        public List<byte> RDATA { get; set; } = new List<byte>();
        public DnsResourceRecord(byte[] Bytes, int Offset)
        {
            Name = new DnsName(Bytes, Offset);

            var TotalOffset = Name.ByteCount + Offset;

            Type = (ushort)((Bytes[TotalOffset] << 8) | Bytes[TotalOffset + 1]);
            Class = (ushort)((Bytes[TotalOffset + 2] << 8) | Bytes[TotalOffset + 3]);
            TTL = (ushort)((Bytes[TotalOffset + 4] << 24) | (Bytes[TotalOffset + 5] << 16)
                | (Bytes[TotalOffset + 6] << 8) | Bytes[TotalOffset + 7]);
            RDLENGTH = (ushort)((Bytes[TotalOffset + 8] << 8) | (Bytes[TotalOffset + 9]));

            ByteCount = TotalOffset + 10;
            for(int i = 0; i < RDLENGTH; i++)
            {
                RDATA.Add(Bytes[ByteCount + i]);
            }

        }
        public DnsResourceRecord(DnsName Name, ushort Type, ushort Class, 
            int TTL, IPAddress Ip)
        {
            this.Name = Name;
            this.Class = Class;
            this.Type = Type;
            this.TTL = TTL;
            RDATA = Ip.GetAddressBytes().ToList();
            RDLENGTH = (ushort)RDATA.Count;
        }
        public byte[] GetBytes()
        {
            var Bytes = new List<byte>();

            Bytes.AddRange(Name.GetBytes());

            Bytes.Add((byte)(Type >> 8));
            Bytes.Add((byte)Type);

            Bytes.Add((byte)(Class >> 8));
            Bytes.Add((byte)Class);

            Bytes.Add((byte)(TTL >> 24));
            Bytes.Add((byte)(TTL >> 16));
            Bytes.Add((byte)(TTL >> 8));
            Bytes.Add((byte)TTL);

            Bytes.Add((byte)(RDLENGTH >> 8));
            Bytes.Add((byte)RDLENGTH);

            Bytes.AddRange(RDATA);

            return Bytes.ToArray();

        }
    }
}