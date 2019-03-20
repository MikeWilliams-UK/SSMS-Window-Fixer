using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SSMS_Window_Fixer
{
    public class Helper
    {
        #region Win32 API

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        private delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("USER32.DLL")]
        private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        private static extern IntPtr GetShellWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        #endregion

        public Process[] GetAllProcesses()
        {
            return Process.GetProcesses();
        }

        public IDictionary<IntPtr, string> GetOpenWindowsFromPid(int processId)
        {
            IntPtr hShellWindow = GetShellWindow();
            Dictionary<IntPtr, string> dictWindows = new Dictionary<IntPtr, string>();

            EnumWindows(delegate (IntPtr hWnd, int lParam)
            {
                if (hWnd == hShellWindow)
                {
                    return true;
                }
                if (!IsWindowVisible(hWnd))
                {
                    return true;
                }

                int length = GetWindowTextLength(hWnd);
                if (length == 0)
                {
                    return true;
                }

                uint windowPid;
                GetWindowThreadProcessId(hWnd, out windowPid);
                if (windowPid != processId)
                {
                    return true;
                }

                StringBuilder stringBuilder = new StringBuilder(length);
                GetWindowText(hWnd, stringBuilder, length + 1);
                dictWindows.Add(hWnd, stringBuilder.ToString());

                return true;
            }, 0);

            return dictWindows;
        }

        public bool ParentIsBiggerThanChild(Rect parent, Rect child)
        {
            bool result = true;

            int parentWidth = Math.Abs(parent.Left - parent.Right);
            int parentHeight = Math.Abs(parent.Top - parent.Bottom);
            int childWidth = Math.Abs(child.Left - child.Right);
            int childHeight = Math.Abs(child.Top - child.Bottom);

            result = parentWidth > childWidth && parentHeight > childHeight;

            return result;
        }

        public bool ChildIsOutsideParent(Rect parent, Rect child)
        {
            bool result = !(parent.Left <= child.Left
                && child.Right <= parent.Right
                && parent.Top <= child.Top
                && child.Bottom <= parent.Bottom);

            return result;
        }

        public Position GetNewPosition(Rect parent, Rect child)
        {
            Position p = new Position();

            int parentWidth = Math.Abs(parent.Left - parent.Right);
            int parentHeight = Math.Abs(parent.Top - parent.Bottom);

            p.Width = Math.Abs(child.Right - child.Left);
            p.Height = Math.Abs(child.Top - child.Bottom);
            p.Left = parent.Left + parentWidth / 2 - p.Width / 2;
            p.Top = parent.Top + parentHeight / 2 - p.Height / 2;

            if (p.Top < parent.Top)
            {
                p.Top = parent.Top;
            }
            if (p.Left < parent.Left)
            {
                p.Left = parent.Left;
            }

            return p;
        }


    }

    public struct Position
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }

        public override string ToString()
        {
            return $"Left: {Left}, Right: {Right}, Top: {Top}, Bottom: {Bottom}";
        }
    }
}
