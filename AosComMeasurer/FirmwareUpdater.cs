using NLog;
using System;
using System.Diagnostics;

namespace AosComMeasurer
{
    public class FirmwareUpdater
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly ProcessStartInfo startInfo;

        public FirmwareUpdater(string comPort, string hexFile)
        {
            try
            {

                startInfo = new ProcessStartInfo
                {
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    FileName = @"ThirdParty\avrdude.exe",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,

                    //avrdude.exe -v -C"avrdude.conf" -pm328p -carduino -PCOM4 -b57600 -Uflash:w:"D:\2.hex":i
                    //Arguments = string.Format("-C\"ThirdParty\\avrdude.conf\" -v -pm328p -carduino -P{0} -b57600 -Uflash:w:\"{1}\":i", comPort, hexFile)
                    Arguments = string.Format("-C\"ThirdParty\\avrdude.conf\" -q -pm328p -carduino -P{0} -b57600 -Uflash:w:\"{1}\":i", comPort, hexFile)
                };


                using (Process exeProcess = Process.Start(startInfo))
                {
                    string error = exeProcess.StandardError.ReadToEnd();
                    string output = exeProcess.StandardOutput.ReadToEnd();
                    logger.Error("FirmwareLoader: output {0}, error {1}", output, error);
                    exeProcess.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "FirmwareLoader: constuctor: filename {0}, arguments {1}", startInfo.FileName, startInfo.Arguments);
            }
        }
    }
}
