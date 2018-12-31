﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace grid
{
	[ToolboxItem(true)]
	[ToolboxBitmap(typeof(ListView))]
	public class ListViewEx : System.Windows.Forms.ListView
	{
		public ListViewEx()
		{
			//Activate double buffering
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

			//Enable the OnNotifyMessage event so we get a chance to filter out 
			// Windows messages before they get to the form's WndProc
			this.SetStyle(ControlStyles.EnableNotifyMessage, true);
		}

		protected override void OnNotifyMessage(Message m)
		{
			//Filter out the WM_ERASEBKGND message
			if (m.Msg != 0x14)
			{
				base.OnNotifyMessage(m);
			}
		}
	}
}
