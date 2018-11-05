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
        public frmMain()
        {
            //Thread t = new Thread(new ThreadStart(StartForm));
            //t.Start();
            //Thread.Sleep(5000);
            InitializeComponent();
           
        }

        public void StartForm()
        {
            Application.Run(new frmSplashScreen());
        }
    }
}
