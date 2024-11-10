using HoiThiDV.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HoiThiDV
{
    public partial class Form1 : Form
    {

        private MasterController ctrl;
        public bool isDisconnected = true;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.ctrl.showTeam5();
        }

        public void setController(MasterController ctrl)
        {
            this.ctrl = ctrl;
        }

        public void showMessage(String msg)
        {
            try
            {
                MessageBox.Show(this, msg);
            }
            catch
            {
                MessageBox.Show("Mất kết nối tới máy chủ!");
            }
            
        }

        public void checkForIllegalThreadCalls(bool b)
        {
            CheckForIllegalCrossThreadCalls = b;
        }

        public void addUserControl(UserControl u)
        {
            if(!this.pnlContent.Controls.Contains(u))
            {
                if (u != null)
                {
                    u.Dock = DockStyle.Fill;
                }
                
                this.BeginInvoke((Action)(() =>
                {
                    //perform on the UI thread
                    this.pnlContent.Controls.Clear();
                    this.pnlContent.Controls.Add(u);
                }));
            }
            
        }
    }
}
