using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace SSMS_Window_Fixer
{
    public partial class Form1 : Form
    {
        private string[] FindWindowTitles = new[]
        {
            "Microsoft SQL Server Management Studio",
            "Microsoft Visual Studio"
        };

        private string[] ExcludeExactTitles = new[] { "Microsoft Visual Studio" };

        public Form1()
        {
            InitializeComponent();

            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Screen[] screens = Screen.AllScreens;
            //foreach (Screen screen in screens)
            //{
            //    Debug.WriteLine($"{screen.WorkingArea}");
            //}
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Helper h = new Helper();
            Process[] procs = h.GetAllProcesses();
            //Debug.WriteLine(procs.Length.ToString());
            foreach (var p in procs)
            {
                if (IsTargetted(p.MainWindowTitle))
                {
                    //Debug.WriteLine($"Found target parent '{p.MainWindowTitle}'");

                    Rect parentRect = new Rect();
                    Helper.GetWindowRect(p.MainWindowHandle, ref parentRect);
                    IDictionary<IntPtr, string> windows = h.GetOpenWindowsFromPid(p.Id);
                    foreach (KeyValuePair<IntPtr, string> kvp in windows)
                    {
                        Rect childRect = new Rect();
                        Helper.GetWindowRect(kvp.Key, ref childRect);

                        if (h.ParentIsBiggerThanChild(parentRect, childRect))
                        {
                            if (h.ChildIsOutsideParent(parentRect, childRect))
                            {
                                Debug.WriteLine($"Moving child '{kvp.Value}'");

                                Position pp = h.GetNewPosition(parentRect, childRect);
                                Helper.MoveWindow(kvp.Key, pp.Left, pp.Top, pp.Width, pp.Height, true);
                            }
                        }
                    }
                }
            }
        }

        private bool IsTargetted(string mainWindowTitle)
        {
            bool result = false;

            if (!string.IsNullOrEmpty(mainWindowTitle))
            {
                bool excluded = false;

                foreach (var title in ExcludeExactTitles)
                {
                    if (mainWindowTitle.ToLower().Equals(title.ToLower()))
                    {
                        excluded = true;
                        break;
                    }
                }

                if (!excluded)
                {
                    foreach (var title in FindWindowTitles)
                    {
                        if (mainWindowTitle.ToLower().Contains(title.ToLower()))
                        {
                            result = true;
                            break;
                        }
                    }
                }

                //Debug.WriteLine(mainWindowTitle + " Targetted: " + result);
            }
            return result;
        }

        //private bool IsOnScreen(Rectangle formRectangle)
        //{
        //    Screen[] screens = Screen.AllScreens;

        //    foreach (Screen screen in screens)
        //    {
        //        if (screen.WorkingArea.Contains(formRectangle))
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}
    }
}