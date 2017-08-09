using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuperClipBoard
{
    public partial class Form1 : Form
    {
        const int mActionOlderClipBoard = 1;
        const int mActionNewerClipBoard = 2;
        const int mActionDeleteRecentClipBoard = 3;

        protected int it = 0;
        protected List<string> myclipboards;

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool RemoveClipboardFormatListener(IntPtr hwnd);



        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int key);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hwnd, int id);


        private const int WM_CLIPBOARDUPDATE = 0x031D;
        public Form1()
        {
            InitializeComponent();

            AddClipboardFormatListener(this.Handle);
            //Modifier keys codes: Alt = 1, Ctrl = 2, Shift = 4, Win = 8
            RegisterHotKey(this.Handle, mActionNewerClipBoard, 1, (int)Keys.N);
            RegisterHotKey(this.Handle, mActionOlderClipBoard, 1, (int)Keys.M);
            RegisterHotKey(this.Handle, mActionDeleteRecentClipBoard, 1, (int)Keys.D);
            this.Visible = false;
            myclipboards = new List<string>();
        }

        protected override void WndProc(ref Message m)
        {

            if (m.Msg == WM_CLIPBOARDUPDATE)
            {
                if (Clipboard.ContainsText())
                {
                    string text = Clipboard.GetText(TextDataFormat.Text);
                    myclipboards.Add(text);
                }
            }

            else if ((m.Msg == 0x0312) && (m.WParam.ToInt32() == mActionOlderClipBoard))
            {
                if (it < myclipboards.Count - 1)
                {
                    ++it;
                }
                else
                {
                    it = 0;
                }

                Clipboard.SetText(myclipboards[it]);
            }

            else if ((m.Msg == 0x0312) && (m.WParam.ToInt32() == mActionNewerClipBoard))
            {
                if (it > 0)
                {
                    --it;
                }
                else
                {
                    it = myclipboards.Count - 1;
                }

                Clipboard.SetText(myclipboards[it]);
            }

            else if ((m.Msg == 0x0312) && (m.WParam.ToInt32() == mActionDeleteRecentClipBoard))
            {
                if (myclipboards.Count > 0)
                {
                    myclipboards.RemoveAt(0);
                }
            }
            else
            {
                base.WndProc(ref m);
            }

        }//end of WndProc





    }
}
