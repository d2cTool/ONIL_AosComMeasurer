using AosComMeasurer;
using System;

namespace ConsoleAppForTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            ComReader comReader = ComReader.Instance;
            comReader.DataReceived += ComReader_DataReceived;
            comReader.LostConnection += ComReader_LostConnection;

            Console.WriteLine("Key: '1' - Init, '2' - Start, 3 - 'Stop'");

            while (true)
            {
                var keyInfo = Console.ReadKey();

                switch (keyInfo.KeyChar)
                {
                    case '1':
                        comReader.Init();
                        break;
                    case '2':
                        comReader.Start();
                        break;
                    case '3':
                        comReader.Stop();
                        break;
                    default:
                        return;
                }
            }
        }

        private static void ComReader_LostConnection(object sender, DataEventArgs e)
        {
            Console.WriteLine($"LostConnection");
        }

        private static void ComReader_DataReceived(object sender, DataEventArgs e)
        {
            Console.WriteLine($"Data recieved: {e.Data}");
        }
    }
}
