using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KillProcess
{
    public partial class FrmKillProcess : Form
    {

        public FrmKillProcess()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            lvProcessList.FullRowSelect = true;
            lvProcessList.Columns.Clear();
            lvProcessList.View = View.Details;
            lvProcessList.Columns.Add("名称", 340, HorizontalAlignment.Left);
            lvProcessList.Items.Clear();

            var ps = Process.GetProcesses();

            foreach (var p in ps)
            {
                if (p.ProcessName.ToLower().Contains(txtKey.Text.ToLower()))
                {
                    lvProcessList.Items.Add(p.ProcessName);
                }
            }
        }

        private void btnKillProcess_Click(object sender, EventArgs e)
        {

            try
            {
                var ps = Process.GetProcessesByName(lvProcessList.SelectedItems[0].Text);
                foreach(var p in ps)
                {
                    p.Kill();
                }
                MessageBox.Show("OK");
                btnSearch.PerformClick();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtKey_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnKillProcess.PerformClick();
            }
        }
    }
}
