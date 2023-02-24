using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WPE
{
public static class Win32
{
    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string className,string winName);

    [DllImport("user32.dll")]
    public static extern IntPtr SendMessageTimeout(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam, uint fuFlage, uint timeout, IntPtr result);

     [DllImport("User32.dll")]

     public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        
    [DllImport("user32.dll")]
    public static extern bool EnumWindows(EnumWindowsProc proc,IntPtr lParam);
    public delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string className, string winName);

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hwnd,int nCmdShow);

    [DllImport("user32.dll")]
    public static extern IntPtr SetParent(IntPtr hwnd,IntPtr parentHwnd);
}
}
