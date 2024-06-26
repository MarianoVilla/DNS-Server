﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafters_dns_server.src
{
    public class DnsHeader
    {
        public ushort ID { get; set; }
        public bool QR { get; set; }
        public byte OPCODE { get; set; }
        public bool AA { get; set; }
        public bool TC { get; set; }
        public bool RD { get; set; }
        public bool RA { get; set; }
        public byte Z { get; set; }
        public byte RCODE { get; set; }
        public byte QDCOUNT { get; set; }
        public byte ANCOUNT { get; set; }
        public byte NSCOUNT { get; set; }
        public byte ARCOUNT { get; set; }
        public DnsHeader(byte[] MessageBytes)
        {
            if (MessageBytes == null)
                throw new ArgumentNullException(nameof(MessageBytes));
            if (MessageBytes.Length < 12)
                throw new ArgumentException(nameof(MessageBytes));

            ID = (ushort)(MessageBytes[0] << 8 | MessageBytes[1]);
            QR = (MessageBytes[2] & 0b10000000) != 0;
            OPCODE = (byte)((MessageBytes[2] & 0b01111000) >> 3);
            AA = (MessageBytes[2] & 0b00000100) != 0;
            TC = (MessageBytes[2] & 0b00000010) != 0;
            RD = (MessageBytes[2] & 0b00000001) != 0;
            RA = (MessageBytes[3] & 0b10000000) != 0;
            Z = (byte)((MessageBytes[3] & 0b01110000) >> 4);
            RCODE = (byte)(MessageBytes[3] & 0b00001111);
            QDCOUNT = (byte)(MessageBytes[4] << 8 | MessageBytes[5]);
            ANCOUNT = (byte)(MessageBytes[6] << 8 | MessageBytes[7]);
            NSCOUNT = (byte)(MessageBytes[8] << 8 | MessageBytes[9]);
            ARCOUNT = (byte)(MessageBytes[10] << 8 | MessageBytes[11]);
        }
        public byte[] GetBytes()
        {
            var Response = new byte[12];
            Response[0] = (byte)(ID >> 8);
            Response[1] = (byte)(ID & 0b11111111);
            Response[2] = (byte)(
                QR.ToInt() << 7 |
                OPCODE << 3 |
                AA.ToInt() << 2 |
                TC.ToInt() << 1 |
                RD.ToInt()
                );

            Response[3] = (byte)(
                RA.ToInt() << 7 |
                Z << 4 |
                RCODE
                );

            Response[4] = (byte)(QDCOUNT >> 8);
            Response[5] = (byte)(QDCOUNT & 0b11111111);

            Response[6] = (byte)(ANCOUNT >> 8);
            Response[7] = (byte)(ANCOUNT & 0b11111111);

            Response[8] = (byte)(NSCOUNT >> 8);
            Response[9] = (byte)(NSCOUNT & 0b11111111);

            Response[10] = (byte)(ARCOUNT >> 8);
            Response[11] = (byte)(ARCOUNT & 0b11111111);

            return Response;
        }
    }
}
