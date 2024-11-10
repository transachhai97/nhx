using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HoiThiDV
{
    public partial class ChangeScore : Form
    {
        public float newScore { get; set; }
        public ChangeScore()
        {
            InitializeComponent();
        }

        public void setLabelChangeScore(string txt)
        {
            this.label1.Text = txt;
        } 
        private void button1_Click(object sender, EventArgs e)
        {
            string newValue = txtNewScore.Text;
            try
            {
                newScore = float.Parse(newValue);
                this.DialogResult = DialogResult.OK;
                this.Close();
            } catch
            {
                MessageBox.Show("Số điểm mới không hợp lệ!");
                txtNewScore.Text = "";
                txtNewScore.Focus();
                return;
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ChangeScore_Load(object sender, EventArgs e)
        {
            txtNewScore.Focus();
        }
    }
}
