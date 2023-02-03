/*
 * 由SharpDevelop创建。
 * 用户： Administrator
 * 日期: 2021/12/13/周一
 * 时间: 2:33
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WPE
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		// 指向 Program Manager 窗口句柄
	    private IntPtr programIntPtr = IntPtr.Zero;
	
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			Process.Start("ffplay.exe ", " *.mp4 -an  -loop 0  -fs");
			Thread.Sleep(1000);
			this.play();
			base.WindowState = FormWindowState.Minimized;
			flag=true;
			this.notifyIcon1.Visible = true;
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		private void play()
		{
			this.Init();
			IntPtr hwnd = Win32.FindWindow("SDL_app", null);
			IntPtr hwnd2 = Win32.FindWindow("ConsoleWindowClass", null);
			Win32.ShowWindow(hwnd2, 0);
			Win32.SetParent(hwnd, this.programIntPtr);
		}
		public void Init()
	    {
	        // 通过类名查找一个窗口，返回窗口句柄。
	        programIntPtr = Win32.FindWindow("Progman", null);
	
	        // 窗口句柄有效
	        if(programIntPtr != IntPtr.Zero)
	        {   
	
	            IntPtr result = IntPtr.Zero;
	
	            // 向 Program Manager 窗口发送 0x52c 的一个消息，超时设置为0x3e8（1秒）。
	            Win32.SendMessageTimeout(programIntPtr, 0x52c, IntPtr.Zero, IntPtr.Zero, 0, 0x3e8, result);
	
	            // 遍历顶级窗口
	            Win32.EnumWindows((hwnd, lParam) =>
	            {
	                // 找到包含 SHELLDLL_DefView 这个窗口句柄的 WorkerW
	                if (Win32.FindWindowEx(hwnd,IntPtr.Zero, "SHELLDLL_DefView", null) != IntPtr.Zero)
	                {
	                    // 找到当前 WorkerW 窗口的，后一个 WorkerW 窗口。 
	                    IntPtr tempHwnd = Win32.FindWindowEx(IntPtr.Zero, hwnd, "WorkerW", null);
	
	                    // 隐藏这个窗口
	                    Win32.ShowWindow(tempHwnd, 0);
	                }
	                return true;
	            }, IntPtr.Zero);
	        }
	    }
		
	 
		const int WM_CLOSE = 0x0010;
		void ExitToolStripMenuItemClick(object sender, EventArgs e)
		{
			//找到控制台窗口
	        IntPtr cmdIntPtr = Win32.FindWindow("ConsoleWindowClass", null);
			Win32.SendMessage(cmdIntPtr, WM_CLOSE, 0, 0);
		 
			this.Close();
		}
		
		void Button1Click(object sender, EventArgs e)
		{
				// 初始化桌面窗口
	            Init();
	            //找到播放器窗口    
	            IntPtr ffplayIntPtr = Win32.FindWindow("SDL_app", null);
	            //找到控制台窗口
	            IntPtr cmdIntPtr = Win32.FindWindow("ConsoleWindowClass", null);
	            //隐藏控制台窗口
	            Win32.ShowWindow(cmdIntPtr, 0);
	            	
	            // 窗口置父，设置背景窗口的父窗口为 Program Manager 窗口
	            Win32.SetParent(ffplayIntPtr, programIntPtr);
	            
	            
	            
		}
		
		void MainFormResize(object sender, EventArgs e)
		{
			if (this.WindowState == FormWindowState.Minimized)
            {
                this.Visible=false;
                this.notifyIcon1.Visible = true;
			}else{
				
                this.notifyIcon1.Visible = false;
			}
		}
		
		void NotifyIcon1MouseDoubleClick(object sender, MouseEventArgs e)
		{
//			this.Visible=true;
//			this.WindowState =FormWindowState.Normal;
		}
		
		 
		
		void MainFormFormClosing(object sender, FormClosingEventArgs e)
		{
			//找到控制台窗口
	        IntPtr cmdIntPtr = Win32.FindWindow("ConsoleWindowClass", null);
			Win32.SendMessage(cmdIntPtr, WM_CLOSE, 0, 0);
		 
			 
		}
		
		/// <summary>
		/// 转换Image为Icon
		/// </summary>
		/// <param name="image">要转换为图标的Image对象</param>
		/// <param name="nullTonull">当image为null时是否返回null。false则抛空引用异常</param>
		/// <exception cref="ArgumentNullException" />
		public static Icon ConvertToIcon(Image image, bool nullTonull = false)
		{
		    if (image == null)
		    {
		        if (nullTonull) { return null; }
		        throw new ArgumentNullException("image");
		    }
		
		    using (MemoryStream msImg = new MemoryStream()
		                      , msIco = new MemoryStream())
		    {
		        image.Save(msImg, ImageFormat.Png);
		
		        using (var bin = new BinaryWriter(msIco))
		        {
		            //写图标头部
		            bin.Write((short)0);           //0-1保留
		            bin.Write((short)1);           //2-3文件类型。1=图标, 2=光标
		            bin.Write((short)1);           //4-5图像数量（图标可以包含多个图像）
		
		            bin.Write((byte)image.Width);  //6图标宽度
		            bin.Write((byte)image.Height); //7图标高度
		            bin.Write((byte)0);            //8颜色数（若像素位深>=8，填0。这是显然的，达到8bpp的颜色数最少是256，byte不够表示）
		            bin.Write((byte)0);            //9保留。必须为0
		            bin.Write((short)0);           //10-11调色板
		            bin.Write((short)32);          //12-13位深
		            bin.Write((int)msImg.Length);  //14-17位图数据大小
		            bin.Write(22);                 //18-21位图数据起始字节
		
		            //写图像数据
		            bin.Write(msImg.ToArray());
		
		            bin.Flush();
		            bin.Seek(0, SeekOrigin.Begin);
		            return new Icon(msIco);
		        }
		    }
		}
		public static Bitmap rotateImage(Bitmap b, float angle)
{
    //create a new empty bitmap to hold rotated image
    Bitmap returnBitmap = new Bitmap(b.Width, b.Height);
    //make a graphics object from the empty bitmap
    Graphics g = Graphics.FromImage(returnBitmap);
    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
    //move rotation point to center of image
    g.TranslateTransform((float)b.Width / 2, (float)b.Height / 2);
    //rotate
    g.RotateTransform(angle);
    //move image back
    g.TranslateTransform(-(float)b.Width / 2, -(float)b.Height / 2);
    //draw passed in image onto graphics object
    g.DrawImage(b, new Point(0, 0));
    return returnBitmap;
}
		
		bool flag=false;
	 async	void  Timer1Tick(object sender, EventArgs e)
		{
	 		Image img=Image.FromFile("小风车.png");
			Bitmap bmp = new Bitmap(img);
			if (flag) {
				
				for (int i = 0; i < 9; i++) {
					notifyIcon1.Icon=ConvertToIcon(rotateImage(bmp,i*(-45)));
					await Task.Delay(125);
				}
				
			}
		}
		void AboutToolStripMenuItemClick(object sender, EventArgs e)
		{
			MessageBox.Show("小风车 ver1.0\n作者：吃爆米花的小熊");
		}
		void DonateToolStripMenuItemClick(object sender, EventArgs e)
		{
			Process.Start("https://partme.com/xiaoxiong");
		}
		//----------------------
	}
}
