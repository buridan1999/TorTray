using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace TorTray
{
    static class Program
    {
        static void Main()
        {
            bool createdNew;
            var mutex = new Mutex(true, "TorTray", out createdNew);
            if (!createdNew)
            {
                MessageBox.Show("TorTray app already running");
                Environment.Exit(0);
            }
            Application.Run(new TrayApp());
        }
    }
    public class TrayApp : ApplicationContext
    {
        //const string TorPath = "\"C:\\Program Files (x86)\\Tor\\Tor Browser\\Browser\\TorBrowser\\Tor\\tor.exe\" - f C:\\tor\\Data\\Tor\\torrc";
        const string TorPath = "\"C:\\Program Files (x86)\\Tor\\Tor Browser\\Browser\\TorBrowser\\Tor\\tor.exe\"";
        const string TorArgs = "-f C:\\tor\\Data\\Tor\\torrc";
        private NotifyIcon notifyIcon;
        //private Process torProcess;
        //private Process[] torProcesses;
        private List<Process> torProcesses;
        private bool torEnabled = false;

        private ContextMenu Restart_;
        private ContextMenu Start_;
        public TrayApp()
        {
            try
            {
                torProcesses = new List<Process>(Process.GetProcessesByName("tor"));
                if (torProcesses.Count > 0)
                {
                    torEnabled = true;
                }
                else
                {
                    ProcessStartInfo StartInfo = new ProcessStartInfo(TorPath);
                    StartInfo.Arguments = TorArgs;

                    //new List<Process>(new int[] { 10, 20, 10, 34, 113 });
                    torProcesses.Add(Process.Start(StartInfo));
                    torEnabled = true;
                }

                Start_ = new ContextMenu(new MenuItem[]
                    {
                        new MenuItem("Start", Restart),
                        new MenuItem("ExitTray", ExitTray)
                    });
                Restart_ = new ContextMenu(new MenuItem[]
                    {
                        new MenuItem("Restart", Restart),
                        new MenuItem("Shutdown", Shutdown),
                        new MenuItem("ExitTray", ExitTray)
                    });

                notifyIcon = null;
                notifyIcon = new NotifyIcon()
                {
                    Icon = Properties.Resources.toricon,
                    ContextMenu = Restart_,
                    Visible = true
                };
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                Environment.Exit(0);
            }
        }
        private void Restart(object sender, EventArgs e)
        {
            foreach (Process proc in torProcesses)
            {
                if (torEnabled)
                    proc.Kill();

                proc.Start();
            }

            torEnabled = true;
            notifyIcon.ContextMenu = Restart_;
        }
        private void Shutdown(object sender, EventArgs e)
        {
            if (torEnabled)
            {
                foreach (Process proc in torProcesses)
                {
                    proc.Kill();
                }
            }

            torEnabled = false;

            notifyIcon.ContextMenu = Start_;
        }

        private void ExitTray(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            Environment.Exit(0);
        }
    }
}
