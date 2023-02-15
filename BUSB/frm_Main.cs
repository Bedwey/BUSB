using System;
using Microsoft.Win32;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace BUSB
{
    public partial class frm_Main : XtraForm
    {
        internal static frm_Main _instance;
        internal static frm_Main Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new frm_Main();
                return _instance;
            }
        }

        public frm_Main()
        {
            InitializeComponent();
        }

        private void frm_Main_Load(object sender, EventArgs e)
        {
            REGStatus();
            USBStatus();
        }

        private void frm_Main_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void REGStatus()
        {
            try
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
                rg_regedit.SelectedIndex = Convert.ToInt16(registryKey.GetValue("DisableRegistryTools", (object)null)) == (short)1 ? 1 : 0;
                registryKey.Close();
            }
            catch (Exception)
            {
                Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies", true).CreateSubKey("System");
            }
        }

        private void USBStatus()
        {
            try
            {
                if (Convert.ToInt16(Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\StorageDevicePolicies").GetValue("WriteProtect", (object)null)) == (short)1)
                    this.rg_usb.SelectedIndex = 1;
                else
                    this.rg_usb.SelectedIndex = 0;
            }
            catch (Exception)
            {
                RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control", true);
                registryKey.CreateSubKey("StorageDevicePolicies");
                registryKey.Close();
            }

            try
            {
                if (Convert.ToInt16(Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\UsbStor").GetValue("Start", (object)null)) != (short)4)
                    return;
                this.rg_usb.SelectedIndex = 2;
            }
            catch (Exception)
            {
                Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services", true).CreateSubKey("USBSTOR");
                RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\UsbStor", true);
                registryKey.SetValue("Type", (object)1, RegistryValueKind.DWord);
                registryKey.SetValue("Start", (object)3, RegistryValueKind.DWord);
                registryKey.SetValue("ImagePath", (object)"system32\\drivers\\usbstor.sys", RegistryValueKind.ExpandString);
                registryKey.SetValue("ErrorControl", (object)1, RegistryValueKind.DWord);
                registryKey.SetValue("DisplayName", (object)"USB Mass Storage Driver", RegistryValueKind.String);
                registryKey.Close();
            }
        }

        private void ChangeUsbState(bool state)
        {
            int val = state ? 3 : 4;
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\UsbStor", true);
            registryKey?.SetValue("Start", (object)val, RegistryValueKind.DWord);
            registryKey.Close();
        }

        private void ChangeRegeditState(bool state)
        {
            int val = state ? 0 : 1;
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", true);
            registryKey.SetValue("DisableRegistryTools", (object)val, RegistryValueKind.DWord);
            registryKey.Close();
        }

        private void ChangeUsbWriteProtect(bool state)
        {
            int val = state ? 1 : 0;
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\StorageDevicePolicies", true);
            if (registryKey == null)
            {
                Registry.LocalMachine.CreateSubKey("SYSTEM\\CurrentControlSet\\Control\\StorageDevicePolicies", RegistryKeyPermissionCheck.ReadWriteSubTree);
                Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\StorageDevicePolicies", true).SetValue("WriteProtect", (object)val, RegistryValueKind.DWord);
            }
            else
            {
                registryKey?.SetValue("WriteProtect", (object)val, RegistryValueKind.DWord);
            }
            registryKey.Close();
        }

        enum USBState
        {
            FullAccess,
            ReadOnly,
            Disabled
        }

        enum REGState
        {
            Enable,
            Disable
        }

        private void btn_Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (XtraMessageBox.Show("Apply changes?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            switch (this.rg_usb.SelectedIndex)
            {
                case (int)USBState.FullAccess:
                    this.ChangeUsbState(true);
                    this.ChangeUsbWriteProtect(false);
                    break;
                case (int)USBState.ReadOnly:
                    this.ChangeUsbState(true);
                    this.ChangeUsbWriteProtect(true);
                    break;
                case (int)USBState.Disabled:
                    this.ChangeUsbState(false);
                    break;
            }

            switch (this.rg_regedit.SelectedIndex)
            {
                case (int)REGState.Enable:
                    this.ChangeRegeditState(true);
                    break;
                case (int)REGState.Disable:
                    this.ChangeRegeditState(false);
                    break;
            }

            XtraMessageBox.Show("In order to enable new setting please reconnect your USB storage devices or restart your computer", "Information", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void btn_Password_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (frm_SetPassword setPassword = new frm_SetPassword())
                setPassword.ShowDialog();
        }

        private void btn_Github_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Bedwey");
        }
    }
}