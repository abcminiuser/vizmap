using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;

namespace FourWalledCubicle.VizMap
{
    [Guid("875170c2-24b5-4280-af37-fe4a27f27f35")]
    public class VisualMapToolWindow : ToolWindowPane
    {
        public VisualMapToolWindow() :
            base(null)
        {
            this.Caption = Resources.ToolWindowTitle;

            this.BitmapResourceID = 301;
            this.BitmapIndex = 1;

            base.Content = new VisualMap();
        }
    }
}
