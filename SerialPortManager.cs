using System;
using System.IO.Ports;
using System.Windows.Forms;


namespace UnsilencedMk2.Serial
{

    public class SerialPortManager : IDisposable
    {
        public SerialPortManager()
        {
            //Gain an overview of the serial ports that are available on the system
            serialProfile.PortNameCollection = SerialPort.GetPortNames();
        }

        private SerialPort serialPort;
        private SerialSettings serialProfile = new SerialSettings();
        private string _latestRecieved = String.Empty;
        //Create an event handler for when new data is recieved from STB.
        public event EventHandler<SerialDataEventArgs> NewSerialDataRecieved;

        //GetSet serial port settings
        public SerialSettings CurrentSerialSettings
        {
            get { return serialProfile; }
            set { serialProfile = value; }
        }


        void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int dataLength = serialPort.BytesToRead;
            byte[] data = new byte[dataLength];
            int nbrDataRead = serialPort.Read(data, 0, dataLength);
            if (nbrDataRead == 0)
                return;
            //Sending the new serial output to 'data' for use later.
            if (NewSerialDataRecieved != null)
                NewSerialDataRecieved(this, new SerialDataEventArgs(data));
        }


        public void StartListening()
        {
            //Closing serial port if it is open
            //Checking for valid configuration
            if (serialPort != null && serialPort.IsOpen)
                serialPort.Close();
            try
            {
                //Set up the serial settings, 57600, 8N1 for Sky Gnome protocol
                serialPort = new SerialPort(
                serialProfile.PortName,
                serialProfile.BaudRate = 57600,
                serialProfile.Parity,
                serialProfile.DataBits = 8,
                serialProfile.StopBits);
            }
            catch (System.ArgumentException)
            {
                MessageBox.Show("Please select the COM port that is connected to the STB ");
                return;
            }



            //Subscribe to event and open serial port for data
            serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
            serialPort.Open();
        }

     
        public void StopListening()
        {
            serialPort.Close();
        }


        //Call to release serial port
        public void Dispose()
        {
            Dispose(true);
        }

        //Dispose process, protected - accessed only by code in same class/derived from
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    serialPort.DataReceived -= new SerialDataReceivedEventHandler(serialPort_DataReceived);
                }
                catch (NullReferenceException)
                //Usually, we'll get this exception if the application is closed without first setting a COM port. This is because, there's simply nothing to dispose yet
                {
                    Application.Exit();
                }
            }
            //Releasing serial port
            if (serialPort != null)
            {
                if (serialPort.IsOpen)
                    serialPort.Close();

                serialPort.Dispose();
            }
        }




    }
    //Event arguments
    public class SerialDataEventArgs : EventArgs
    {
        public SerialDataEventArgs(byte[] dataInByteArray)
        {
            Data = dataInByteArray;
        }

        public byte[] Data;
    }
}
