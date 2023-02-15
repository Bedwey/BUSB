using System;
using Microsoft.Win32;
using System.Windows.Forms;

namespace BUSB
{
    static class Program
    {
        public readonly static string RegistryPath = @"Software\Bedwey Dev\BUSB";

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (RegistryKey registry = Registry.LocalMachine.OpenSubKey(RegistryPath))
            {
                if (registry != null && registry.GetValue("Password") != null)
                    new frm_Login().Show();
                else
                    new frm_Main().Show();
            }

            Application.Run();
        }
    }
}
