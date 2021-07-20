
using System;
using System.IO;
using System.Windows.Forms;

namespace AESEncDecTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string outputFolder = "OUTPUT";
        private string encryptedFolder = "";
        private string decryptedFolder = "";

        private void Form1_Load(object sender, EventArgs e)
        {
            if (IsProgramRunning("AESEncDecTool.exe") > 1)
            {
                MessageBox.Show("Another instance is running", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.ExitThread();
                Environment.Exit(0);
            }

            encryptedFolder = string.Format(@"{0}\ENCRYPTED", outputFolder);
            decryptedFolder = string.Format(@"{0}\DECRYPTED", outputFolder);
            if (!Directory.Exists(outputFolder)) Directory.CreateDirectory(outputFolder);
            if (!Directory.Exists(encryptedFolder)) Directory.CreateDirectory(encryptedFolder);
            if (!Directory.Exists(decryptedFolder)) Directory.CreateDirectory(decryptedFolder);
        }

        private static int IsProgramRunning(string Program)
        {
            System.Diagnostics.Process[] p;
            p = System.Diagnostics.Process.GetProcessesByName(Program.Replace(".exe", "").Replace(".EXE", ""));

            return p.Length;
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            EncryptionDecryptionProcess(1);
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            EncryptionDecryptionProcess(2);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtInput.Text = ofd.FileName;
            }
            ofd.Dispose();
            ofd = null;
        }

        private void EncryptionDecryptionProcess(short intProcess)
        {
            if (txtInput.Text == "")
            {
                MessageBox.Show("Please enter valid file", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (!File.Exists(txtInput.Text))
            {
                MessageBox.Show("Please enter valid file", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MemuFCR_EncDec.EncDec ed = new MemuFCR_EncDec.EncDec(txtKey.Text);
            string inputData = File.ReadAllText(txtInput.Text);            
            bool response=false;
            string process = "Encryption";
            ed.InputData = inputData;
            if (intProcess == 1) response = ed.EncryptData();
            if (intProcess == 2) {
                response = ed.DecryptData();
                process = "Decryption";
            }
            if (response)
            {
                string outputFile = string.Format(@"{0}\{1}", encryptedFolder, Path.GetFileName(txtInput.Text));
                if (intProcess == 2) outputFile = string.Format(@"{0}\{1}", decryptedFolder, Path.GetFileName(txtInput.Text));                
                File.WriteAllText(outputFile, ed.OutputData);
                MessageBox.Show(process + " process is success!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (intProcess == 1)System.Diagnostics.Process.Start(string.Format(@"{0}", encryptedFolder));
                if (intProcess == 2) System.Diagnostics.Process.Start(string.Format(@"{0}", decryptedFolder));
            }
            else MessageBox.Show(process + " process failed [" + ed.ErrorMessage + "]", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            ed = null;
        }        
    }
}
