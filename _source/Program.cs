using System;
using System.Threading;
using System.Windows.Forms;

namespace trajectorx
{
    internal static class Program
    {

        private static readonly string MutexName = "trajectorx-software-mutex";

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.EnableVisualStyles();
            Application.Run(new Interface());
        }
    }
}