using KNXLib.Enums;

namespace KNXLib
{
    public class KnxControlField1
    {
        // Bit order
        // +---+---+---+---+---+---+---+---+
        // | 7 | 6 | 5 | 4 | 3 | 2 | 1 | 0 |
        // +---+---+---+---+---+---+---+---+

        //  Control Field 1

        //   Bit  |
        //  ------+---------------------------------------------------------------
        //    7   | Frame Type  - 0x0 for extended frame
        //        |               0x1 for standard frame
        //  ------+---------------------------------------------------------------
        //    6   | Reserved
        //        |
        //  ------+---------------------------------------------------------------
        //    5   | Repeat Flag - 0x0 repeat frame on medium in case of an error
        //        |               0x1 do not repeat
        //  ------+---------------------------------------------------------------
        //    4   | System Broadcast - 0x0 system broadcast
        //        |                    0x1 broadcast
        //  ------+---------------------------------------------------------------
        //    3   | Priority    - 0x0 system
        //        |               0x1 normal (also called alarm priority)
        //  ------+               0x2 urgent (also called high priority)
        //    2   |               0x3 low
        //        |
        //  ------+---------------------------------------------------------------
        //    1   | Acknowledge Request - 0x0 no ACK requested
        //        | (L_Data.req)          0x1 ACK requested
        //  ------+---------------------------------------------------------------
        //    0   | Confirm      - 0x0 no error
        //        | (L_Data.con) - 0x1 error
        //  ------+---------------------------------------------------------------


        public KnxTelegramType TelegramType { get; private set; } = KnxTelegramType.ExtendedFrame;
        public KnxTelegramRepetitionStatus TelegramRepetitionStatus { get; private set; } = KnxTelegramRepetitionStatus.Repeated;
        public KnxTelegramPriority TelegramPriority { get; private set; } = KnxTelegramPriority.System;

        internal KnxControlField1(byte value)
        {
            Parse(value);
        }

        internal KnxControlField1(KnxTelegramType telegramType, KnxTelegramRepetitionStatus telegramRepetitionStatus, KnxTelegramPriority telegramPriority)
        {
            TelegramType = telegramType;
            TelegramRepetitionStatus = telegramRepetitionStatus;
            TelegramPriority = telegramPriority;
        }

        public byte GetValue()
        {
            // Fixed bits (according to https://support.knx.org/hc/en-us/articles/115003188429-Control-Field)
            // D6 = 0
            // D4 = 1
            // D1 = 0
            // D0 = 0
            byte result = 0b0001_0000;

            if (TelegramType == KnxTelegramType.StandardFrame)
                result = (byte)(result | 0b1000_0000);

            if (TelegramRepetitionStatus == KnxTelegramRepetitionStatus.Original)
                result = (byte)(result | 0b0010_0000);

            if (TelegramPriority == KnxTelegramPriority.Low)
                result = (byte)(result | 0b0000_1100);
            else if (TelegramPriority == KnxTelegramPriority.Urgent)
                result = (byte)(result | 0b0000_1000);
            else if (TelegramPriority == KnxTelegramPriority.Normal)
                result = (byte)(result | 0b0000_1100);

            return result;
        }

        private void Parse(byte value)
        {
            // see https://support.knx.org/hc/en-us/articles/115003188429-Control-Field

            // D7
            // 0 = Extended Frame (9..262 octets)
            // 1 = Standard Frame (8..23 octets)
            if ((value & 0b1000_0000) != 0)
                TelegramType = KnxTelegramType.StandardFrame;

            // D5
            // 0 = Repeated
            // 1 = Not repeated (original)
            if ((value & 0b0010_0000) != 0)
                TelegramRepetitionStatus = KnxTelegramRepetitionStatus.Original;

            // D3 + D2
            // 00 = System
            // 10 = Urgent
            // 01 = Normal
            // 11 = Low
            if ((value & 0b0000_1000) != 0 && (value & 0b0000_0100) != 0)
                TelegramPriority = KnxTelegramPriority.Low;
            else if ((value & 0b0000_1000) != 0)
                TelegramPriority = KnxTelegramPriority.Urgent;
            else if ((value & 0b0000_0100) != 0)
                TelegramPriority = KnxTelegramPriority.Normal;
        }

        public override string ToString()
        {
            return $"Type: {TelegramType.ToString()} // Repeated: {TelegramRepetitionStatus.ToString()} // Priority: {TelegramPriority.ToString()}";
        }
    }
}
