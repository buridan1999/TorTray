using System;
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
        private NotifyIcon notifyIcon;
        private Process torProcess;
        public TrayApp()
        {
            try
            {
                if (Process.GetProcessesByName("tor").Length > 0)
                {
                    MessageBox.Show("tor process already running");
                    Environment.Exit(0);
                }
                torProcess = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = "tor.exe",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                torProcess.Start();
                notifyIcon = new NotifyIcon()
                {
                    Icon = Properties.Resources.toricon,
                    ContextMenu = new ContextMenu(new MenuItem[]
                    {
                        new MenuItem("Restart", Restart),
                        new MenuItem("Shutdown", Shutdown)
                    }),
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
            torProcess.Kill();
            torProcess.Start();
        }
        private void Shutdown(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            torProcess.Kill();
            Environment.Exit(0);
        }
    }
}
