using System;
using System.Text;
using UnsilencedMk2.Serial;
using System.Timers;

namespace UnsilencedMk2
{
    public class AudioControl
    {
        public static SerialPortManager serialPortController;
        static System.Timers.Timer settleTimer;
        public static System.Timers.Timer userDefinedDelay;
        static string serialDataBuffer;

        public static void StartMonitoring()

        {
            //When monitoring starts, set up the user's desired delay.
            userDefinedDelay.Elapsed += UserDelay;
            userDefinedDelay.AutoReset = false;
            userDefinedDelay.Enabled = false;

        }
        public static void serialPortController_NewSerialDataRecieved(object sender, SerialDataEventArgs e)
        {
            //Creating a string buffer so that the information from the STB can be stored and analysed
            int maxTextLength = 1000;

            string str = Encoding.ASCII.GetString(e.Data);

            serialDataBuffer = serialDataBuffer + str;
            if (serialDataBuffer.Length > maxTextLength)
            {
                serialDataBuffer = "";
            }
            settleTimer = new System.Timers.Timer(600);
            // We're going to wait some time to prevent accidential activation when the user quickly enters/exits the EPG
            settleTimer.Elapsed += StartActivation;
            settleTimer.AutoReset = true;
            settleTimer.Enabled = true;
            settleTimer.Start();
        }



        public static void StartActivation(Object source, ElapsedEventArgs e)
        {
            if (serialDataBuffer.Contains("SYFS0081") == true)
            {
                //If the EPG is present onscreen, detect the string output and wait for the user's delay.
                userDefinedDelay.Enabled = true;
                userDefinedDelay.Start();
            }

            if (serialDataBuffer.Contains("SYFS0080") == true)
            {
                //If the EPG is NOT present onscreen, mute the playback
                Program.MainWindow.tsStatus.Text = "Audio Muted";
                serialDataBuffer = "";
                Program.MainWindow.axWindowsMediaPlayer1.settings.volume = 0;
            }
        }




        public static void UserDelay(Object source, ElapsedEventArgs e)
        {
            if (serialDataBuffer.Contains("SYFS0081") == true)
            {
                //Being extra sure that the EPG is definitely present still, to prevent acciental activations
                if (serialDataBuffer.Contains("SYFS0080") == true)
                {
                    return;
                }
                Program.MainWindow.tsStatus.Text = "Audio Unmuted";
                //Unmute the audio
                serialDataBuffer = "";
                Program.MainWindow.axWindowsMediaPlayer1.settings.volume = 100;
                userDefinedDelay.Stop();
                userDefinedDelay.Enabled = false;
            }


        }
    }
}

