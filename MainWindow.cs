using System;
using System.Windows.Forms;
using UnsilencedMk2.Serial;

namespace UnsilencedMk2
{
    public partial class MainWindow : Form
    {
        SerialPortManager serialPortController;

        public MainWindow()
        {
            InitializeComponent();
            UserInitialization();
        }
        private void UserInitialization()
        {
            tsStatus.Text = "Waiting for STB...";
            //Since we're displaying the available COM ports, we initialise the port controller here for future use.
            serialPortController = new SerialPortManager();
            SerialSettings mySerialSettings = serialPortController.CurrentSerialSettings;
            serialSettingsBindingSource.DataSource = mySerialSettings;
            //Bind list of available COM ports for use.
            portNameComboBox.DataSource = mySerialSettings.PortNameCollection;
            serialPortController.NewSerialDataRecieved += new EventHandler<SerialDataEventArgs>(serialPortController_NewSerialDataRecieved);
            this.FormClosing += new FormClosingEventHandler(MainWindow_FormClosing);

        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Properly dispose of the controller when closed.
            serialPortController.Dispose();
        }

        void serialPortController_NewSerialDataRecieved(object sender, SerialDataEventArgs e)
        {
            if (this.InvokeRequired)
            {
                //Avoid two-thread deadlock once closing serial port. 
                this.BeginInvoke(new EventHandler<SerialDataEventArgs>(AudioControl.serialPortController_NewSerialDataRecieved), new object[] { sender, e });
                return;
            }
        }


        private void startDetection_Click(object sender, EventArgs e)
        {
            //Firstly, we validate the user's input into the defined delay field.
            try
            {
                AudioControl.userDefinedDelay = new System.Timers.Timer(Convert.ToInt32(delayInput.Text));
            }
            catch (System.FormatException)
            {
                MessageBox.Show("Please type a non-zero value into the 'Delay' field.");
                return;
            }
            catch (System.ArgumentException)
            {
                MessageBox.Show("Please type a non-zero value into the 'Delay' field.");
                return;
            }

            //Now listening to serial port, ready for activations
            serialPortController.StartListening();
            AudioControl.StartMonitoring();

        }

        private void exit_Click(object sender, EventArgs e)
        {
            serialPortController.Dispose();
            Application.Exit();
        }

        private void browseAudio_Click(object sender, EventArgs e)
        {
            //Provide a Windows Media control and file browsing for users to select desired media to use on EPG.
            openFileDialog1.Filter = "(mp3,wav,mp4,mov,wmv,mpg)|*.mp3;*.wav;*.mp4;*.mov;*.wmv;*.mpg|all files|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            axWindowsMediaPlayer1.URL = openFileDialog1.FileName;
            //Constantly loop said media
            axWindowsMediaPlayer1.settings.setMode("loop", true);
        }

        private void aboutBtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Unsilenced Mk2 v1.0 2021 Alistair Brazier/Chelmsford IT");
        }
    }
}
