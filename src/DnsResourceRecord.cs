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
        public DnsResourceRecord(byte[] Bytes)
        {
            Name = new DnsName(Bytes, 0);

            Type = (ushort)((Bytes[Name.ByteCount] << 8) | Bytes[Name.ByteCount + 1]);
            Class = (ushort)((Bytes[Name.ByteCount + 2] << 8) | Bytes[Name.ByteCount + 3]);
            TTL = (ushort)((Bytes[Name.ByteCount + 4] << 24) | (Bytes[Name.ByteCount + 5] << 16)
                | (Bytes[Name.ByteCount + 6] << 8) | Bytes[Name.ByteCount + 7]);
            RDLENGTH = (ushort)((Bytes[Name.ByteCount + 8] << 8) | (Bytes[Name.ByteCount + 9]));

            ByteCount = Name.ByteCount + 10;
            for(int i = 0; i < RDLENGTH; i++)
            {
                RDATA.Add(Bytes[ByteCount + i]);
            }

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