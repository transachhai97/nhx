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
    public partial class More : Form
    {
        public String type { get; set; }
        public String quest { get; set; }
        public More()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            type = cbType.Text;
            quest = txtQuest.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ChangeScore_Load(object sender, EventArgs e)
        {
            txtQuest.Focus();
        }
    }
}
