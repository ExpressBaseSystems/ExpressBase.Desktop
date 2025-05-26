using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    internal class Util
    {
        private const string MESSAGE_CAPTION = "EXPRESSbase";

        public const string USERID_NUMBER = "number";
        public const string USERID_TEXT = "text";

        internal static void MessageError(string message)
        {
            MessageBox.Show(message, MESSAGE_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        internal static void MessageInfomation(string message)
        {
            MessageBox.Show(message, MESSAGE_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        internal static void MessageStop(string message)
        {
            MessageBox.Show(message, MESSAGE_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        internal static DialogResult MessageQuestion(string message)
        {
            return MessageBox.Show(message, MESSAGE_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }
    }
}
