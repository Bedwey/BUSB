using System;
using Microsoft.Win32;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace BUSB
{
    public partial class frm_Login : XtraForm
    {
        public frm_Login()
        {
            InitializeComponent();
        }

        private void btn_Login_Click(object sender, EventArgs e)
        {
            using (RegistryKey registry = Registry.LocalMachine.OpenSubKey(Program.RegistryPath))
            {
                if (!CommonMethods.ConfirmPwd(edt_Password.Text, registry.GetValue("Password").ToString()))
                {
                    XtraMessageBox.Show("Password Is Incorrect", "Information", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }
            }

            frm_Main.Instance.Show();
            this.Close();
        }

        private void frm_Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (frm_Main._instance == null) Application.Exit();
        }
    }
}