using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Client
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] MyArgs)
        {
//            bool bAllow = true;

//#if (!DEBUG)
//            bAllow = (MyArgs != null && MyArgs.Length == 1 && MyArgs[0] == Constants.SYS_ALLOW + Constants.SYS_LAUNCH);
//#endif
//            if (bAllow)
//            {

//#if Evaluation
//                MessageBox.Show("Trial Version Only");
//#endif

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Login());
            //}
            //else
            //{
            //    MessageBox.Show("ERROR 007", "PHX+", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    Application.Exit();
            //}
        }
    }
}
