using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FourWalledCubicle.VizMap
{
    public partial class VisualMap : UserControl
    {
        private bool _isZooming = false;

        public VisualMap()
        {
            InitializeComponent();

            VisualAddressCanvas.SymbolInfo testSym1 = new VisualAddressCanvas.SymbolInfo();
            testSym1.StartAddress = 0x1;
            testSym1.Size = 1024;
            vacVisualMapView.Symbols.Add(testSym1);

            svMapScroller.KeyDown += vacVisualMapView_KeyChange;
            svMapScroller.KeyUp += vacVisualMapView_KeyChange;
            svMapScroller.PreviewMouseWheel += vacVisualMapView_MouseWheel;
        }

        void vacVisualMapView_KeyChange(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl)
                _isZooming = e.IsDown;
        }

        void vacVisualMapView_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_isZooming)
            {
                e.Handled = true;
                vacVisualMapView.Zoom += (e.Delta > 0 ? 1 : -1);
            }
        }
    }
}