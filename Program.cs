using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;


namespace UnsilencedMk2
{
    static class Program
    {
        
        public static MainWindow MainWindow = new MainWindow(); 
        [STAThread]

        static void Main()
        {
            Application.EnableVisualStyles();
            Application.Run(MainWindow);
        }
    }
}
