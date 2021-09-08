using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.ComponentModel;

namespace UnsilencedMk2.Serial
{
    //Set up and provide params for serial ports
    public class SerialSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        string _portName = "";
        string[] _portNameCollection;
        int _baudRate = 57600;
        Parity _parity = Parity.None;
        StopBits _stopBits = StopBits.One;


        public string PortName
        {
            get { return _portName; }
            set
            {
                _portName = value;
            }
        }

        public int BaudRate
        {
            get { return _baudRate; }
            set
            {
                _baudRate = value;
            }
        }


        public Parity Parity
        {
            get { return _parity; }
            set
            {
                _parity = value;
            }
        }

        public int DataBits;

        public StopBits StopBits
        {
            get { return _stopBits; }
        }

        public string[] PortNameCollection
        {
            get { return _portNameCollection; }
            set { _portNameCollection = value; }
        }

        public int[] DataBitsCollection;
    }
}
