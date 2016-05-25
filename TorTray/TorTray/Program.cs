using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TorTray
{
    static class Program
    {
        static void Main()
        {
            Application.Run(new TrayApp());
        }
    }
    public class TrayApp : ApplicationContext
    {
        private NotifyIcon notifyIcon;
        private Process torProcess = new Process();
        public TrayApp()
        {
            try
            {
                torProcess.StartInfo.FileName = @"tor.exe";
                torProcess.StartInfo.UseShellExecute = false;
                torProcess.StartInfo.CreateNoWindow = true;
                torProcess.Start();

                notifyIcon = new NotifyIcon()
                {
                    Icon = Properties.Resources.toricon,
                    ContextMenu = new ContextMenu(new MenuItem[] {
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
