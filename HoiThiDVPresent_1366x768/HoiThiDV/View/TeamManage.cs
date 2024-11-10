﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HoiThiDV.Model;

namespace HoiThiDV.View
{
    public partial class TeamManage : UserControl
    {
        private MasterController ctrl;
        public TeamManage()
        {
            InitializeComponent();
        }

        public void setController(MasterController ctrl)
        {
            this.ctrl = ctrl;
        }
        public void setData(DataServer data)
        {
            for (int i = 1; i <= 5; i++)
            {
                if (i <= data.ContestTeams.Count)
                {
                    Label txt = (Label)this.Controls.Find("R0_Team" + i, true)[0];
                    txt.Text = "";
                }
                else
                {
                    CompDisableVisible("R0_Team" + i, false);
                    CompDisableVisible("pictureBox5", false);
                }
            }
            if (data != null)
            {
                if (data.ContestTeams != null)
                {
                    foreach (Contestant contest in data.ContestTeams)
                    {
                        Label txt = (Label)this.Controls.Find("R0_Team" + contest.Id, true)[0];
                        txt.Text = contest.Name;
                    }
                }
            }
        }

        private void TeamManage_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
        }
        public void CompDisableVisible(string compName, bool flag)
        {
            Control comp = (Control)this.Controls.Find(compName, true).FirstOrDefault();
            comp.Visible = flag;
        }
    }
}