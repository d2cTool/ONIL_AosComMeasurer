namespace AosComMeasurer
{
    public class MeasurerState
    {
        public string Type { get; set; }
        public string Limit { get; set; }
        public string Btn { get; set; } // 0- , 1- , 2- 
        public string Value { get; set; }

        public byte I0 { get; set; }
        public byte I1 { get; set; }
        public byte I2 { get; set; }
        public byte I3 { get; set; }
        public byte I4 { get; set; }
        public byte I5 { get; set; }
        public byte I6 { get; set; }
        public byte I7 { get; set; }
        public byte I8 { get; set; }
        public byte I9 { get; set; }
        public byte I10 { get; set; }
        public byte I11 { get; set; }
        public byte I12 { get; set; }
        public byte I13 { get; set; }  //probe guard (1 - probe ON, 0 - probe OFF)
        public byte I14 { get; set; }  //burn guard (1 - ON, for OFF user should push button "Btn")
        public byte I15 { get; set; }

        public string Serial { get; set; }
        public int FirmwareVersion { get; set; }
        public string Vcc { get; set; }
    }

    public class BoardState
    {
        public int Minus { get; set; }
        public int Plus { get; set; }
        public string Serial { get; set; }
    }
}
