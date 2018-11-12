using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace LoadingBar
{
    public partial class frmMain : Form
    {
        public int percent { get; set; }
        public frmMain()
        {
            InitializeComponent();
        }

        public void StartForm()
        {

            Application.Run(new frmSplashScreen());
        }
    }
}
