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
        System.Threading.Timer TheTimer = null;
        public bool isDisconnected = true;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.ctrl.showTeam5();
            TheTimer = new System.Threading.Timer(this.Tick, null, 0, 1000);
        }

        public void setController(MasterController ctrl)
        {
            this.ctrl = ctrl;
        }

        public void showMessage(String msg)
        {
            MessageBox.Show(this, msg);
        }

        public void checkForIllegalThreadCalls(bool b)
        {
            CheckForIllegalCrossThreadCalls = b;
        }

        public void addUserControl(UserControl u)
        {
            if(!this.pnlContent.Controls.Contains(u))
            {
                u.Dock = DockStyle.Fill;
                this.BeginInvoke((Action)(() =>
                {
                    //perform on the UI thread

                    this.pnlContent.Controls.Clear();
                    this.pnlContent.Controls.Add(u);
                }));
            }
            
        }
        public void Tick(object info)
        {
            if(isDisconnected)
            {
                this.ctrl.ReconnectServer();
            }
            isDisconnected = true;
            this.ctrl.PingServer();
        }

        private void pnlContent_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
