using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System.Threading;

namespace AeroDataLogger.Output
{
    internal static class StatusLed
    {
        private static OutputPort _statusLed = new OutputPort(Pins.GPIO_PIN_D4, false);
        private static Timer _timer = new Timer(FlashLoop, null, -1, 250);

        public static void Off()
        {
            StopFlash();
            _statusLed.Write(false);
        }

        public static void On()
        {
            StopFlash();
            _statusLed.Write(true);
        }

        public static void Flash()
        {
            _timer.Change(0, 250);
        }

        private static void FlashLoop(object state)
        {
            _statusLed.Write(!_statusLed.Read()); 
        }

        private static void StopFlash()
        {
            _timer.Change(-1, 250);
        }
    }
}
