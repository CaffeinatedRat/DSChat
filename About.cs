using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace DSChat
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
            lblVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
