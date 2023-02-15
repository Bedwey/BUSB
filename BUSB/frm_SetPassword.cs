using Microsoft.Win32;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace BUSB
{
    public partial class frm_SetPassword : XtraForm
    {
        public frm_SetPassword()
        {
            InitializeComponent();
        }

        private void btn_Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (edt_Password.Text != edt_RePassword.Text)
            {
                XtraMessageBox.Show("Password Is Mismatch", "Information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else
            {
                RegistryKey registry = Registry.LocalMachine.OpenSubKey(Program.RegistryPath);
                if (registry == null) registry = Registry.LocalMachine.CreateSubKey(Program.RegistryPath);

                if (edt_Password.Text.Trim() == string.Empty)
                    registry.DeleteValue("Password");
                else
                    registry.SetValue("Password", CommonMethods.MD5Hash(edt_Password.Text));
            }

            this.Close();
        }
    }
}