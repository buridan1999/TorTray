using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using System.Configuration;

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
            Application.Run(new TrayApp(Properties.Settings.Default));
        }
    }
    public class TrayApp : ApplicationContext
    {
        private string TorPath;
        private string TorArgs;

        private NotifyIcon notifyIcon;
        private List<Process> torProcesses;
        private bool torEnabled = false;

        private ContextMenu Restart_;
        private ContextMenu Start_;
        public TrayApp(Properties.Settings settings)
        {
            try
            {
                TorPath = settings.torPath;
                TorArgs = settings.torArgs;

                torProcesses = new List<Process>(Process.GetProcessesByName("tor"));
                if (torProcesses.Count > 0)
                {
                    torEnabled = true;
                }
                else
                {
                    ProcessStartInfo StartInfo = new ProcessStartInfo(TorPath);
                    StartInfo.Arguments = TorArgs;

                    torProcesses.Add(Process.Start(StartInfo));
                    torEnabled = true;
                }

                // menu when tor doesn't started
                Start_ = new ContextMenu(new MenuItem[]
                    {
                        new MenuItem("Start", Restart),
                        new MenuItem("ExitTray", ExitTray)
                    });

                // menu when tor already started
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
