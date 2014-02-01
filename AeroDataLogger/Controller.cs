using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using AeroDataLogger.Output;
using System.Threading;

namespace AeroDataLogger
{
    internal class Controller
    {
        private InterruptPort _button;

        private bool _ready;
        private bool _handlingButtonPress;

        private object _lock = new object();

        public bool Recording { get; private set; }

        public Controller()
        {
            _ready = false;
            _handlingButtonPress = false;
            Recording = false;
            StatusLed.Off();

            _button = new InterruptPort(Pins.GPIO_PIN_D7, false, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeHigh);
            _button.OnInterrupt += new NativeEventHandler(OnButtonPress);
        }

        public void NotifyReady()
        {
            StatusLed.On();
            _ready = true;
        }

        private void OnButtonPress(uint data1, uint data2, DateTime time)
        {
            if (!_ready) return; // pressing the button before the unit is ready will do nothing
            if (_handlingButtonPress) return;  // if this gets called multiple times in succession, ignore the repeats

            lock (_lock)
            {
                _handlingButtonPress = true;
                _button.Interrupt = Port.InterruptMode.InterruptNone; // does this suppress further interrupts?

                Recording = !Recording; // Flip the recording flag
                if (Recording)
                {
                    StatusLed.Flash();
                }
                else
                {
                    StatusLed.Off();
                }

                Thread.Sleep(250); // glitch filtering
                _button.Interrupt = Port.InterruptMode.InterruptEdgeHigh;
                _handlingButtonPress = false;
            }
        }
    }
}
