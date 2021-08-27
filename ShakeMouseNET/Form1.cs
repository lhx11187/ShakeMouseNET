using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ShakeMouseNET
{
    public partial class Form1 : Form
    {
        public const int WM_LBUTTONDOWN = 513; // 鼠标左键按下  
        public const int WM_LBUTTONUP = 514; // 鼠标左键抬起  
        public const int WM_RBUTTONDOWN = 516; // 鼠标右键按下  
        public const int WM_RBUTTONUP = 517; // 鼠标右键抬起  
        public const int WM_MBUTTONDOWN = 519; // 鼠标中键按下  
        public const int WM_MBUTTONUP = 520; // 鼠标中键抬起  

        public const int MOUSEEVENTF_MOVE = 0x0001; // 移动鼠标         
        public const int MOUSEEVENTF_LEFTDOWN = 0x0002; // 鼠标左键按下        
        public const int MOUSEEVENTF_LEFTUP = 0x0004; // 鼠标左键抬起        
        public const int MOUSEEVENTF_RIGHTDOWN = 0x0008; // 鼠标右键按下       
        public const int MOUSEEVENTF_RIGHTUP = 0x0010; // 鼠标右键抬起          
        public const int MOUSEEVENTF_MIDDLEDOWN = 0x0020; // 鼠标中键按下    
        public const int MOUSEEVENTF_MIDDLEUP = 0x0040; // 鼠标中键抬起           
        public const int MOUSEEVENTF_ABSOLUTE = 0x8000; // 绝对坐标 

        public static bool start = true;
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, int lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr WindowFromPoint(int xPoint, int yPoint);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetCursorPos(int x, int y);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool ScreenToClient(IntPtr hWnd, ref Point lppt);

        // [DllImport("user32", SetLastError = true)]  
        // public static extern int mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        [DllImport("user32", SetLastError = true)]  
        public static extern int mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("kernel32.dll")]
        static extern uint SetThreadExecutionState(uint esFlags);
        // 选项所用到的常数
        const uint ES_AWAYMODE_REQUIRED = 0x00000040;
        const uint ES_CONTINUOUS = 0x80000000;
        const uint ES_DISPLAY_REQUIRED = 0x00000002;
        const uint ES_SYSTEM_REQUIRED = 0x00000001;
        public Form1()
        {
            InitializeComponent();
            //this.Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //this.Visible = false;
            this.ShowInTaskbar = false;

            this.WindowState = FormWindowState.Minimized;
            this.mainNotifyIcon.Visible = true;
            this.Hide();
            Thread thread = new Thread(new ThreadStart(ThreadMouse));
            thread.Start();

        }

        static void ThreadMouse()
        {
            /*
              这个没什么东西的，主要就这一句：
            Mouse_Event(MOUSEEVENTF_MOVE,0,0,0,0); 
            也可以用这个：SetThreadExecutionState(ES_CONTINUOUS or ES_SYSTEM_REQUIRED or ES_DISPLAY_REQUIRED) ;
            兼容性应该更好一些。
            */

            while (true)
            {
                if (start)
                { 
                    mouse_event(MOUSEEVENTF_MOVE, 0, 0, 0, 0);
                    SetThreadExecutionState(ES_CONTINUOUS);
                }
                Thread.Sleep(30*1000);
            }
        }

        private void mainNotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.Visible)
            {
                this.WindowState = FormWindowState.Minimized;
                this.mainNotifyIcon.Visible = true;
                this.Hide();
            }
            else
            {
                this.Visible = true;
                this.WindowState = FormWindowState.Normal;
                this.Activate();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 注意判断关闭事件reason来源于窗体按钮，否则用菜单退出时无法退出!
            if (e.CloseReason == CloseReason.UserClosing)
            {
                //取消"关闭窗口"事件
                e.Cancel = true; // 取消关闭窗体 

                //使关闭时窗口向右下角缩小的效果
                this.WindowState = FormWindowState.Minimized;
                this.mainNotifyIcon.Visible = true;
                this.Hide();
                return;
            }
        }

        private void 关闭ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("你确定要退出？", "系统提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {

                this.mainNotifyIcon.Visible = false;
                this.Close();
                this.Dispose();
                System.Environment.Exit(System.Environment.ExitCode);

            }
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.mainNotifyIcon.Visible = true;
            this.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://cfblog.blackduck.workers.dev/");
        }

        private void mainNotifyIcon_Click(object sender, EventArgs e)
        {

            if (start)
            { 
                start = false;
                this.mainNotifyIcon.Icon= Properties.Resources._2;
                this.mainNotifyIcon.Text = "已经停止";
            }
            else
            {
                start = true;
                this.mainNotifyIcon.Icon = Properties.Resources._1;
                this.mainNotifyIcon.Text = "正在运行";
            }
               
        }
    }
}
