using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Board
{
    class Program
    {
        [STAThread]
        public static void Main(String[] args)
        {

            frmBoard D = new frmBoard();

            Application.Run(D);
        }
    }
}
