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
            testSym1.Address = 0x1234;
            testSym1.Size = 50;
            vacVisualMapView.Symbols.Add(testSym1);

            svMapScroller.KeyDown += new KeyEventHandler(vacVisualMapView_KeyDown);
            svMapScroller.KeyUp += new KeyEventHandler(vacVisualMapView_KeyUp);
            svMapScroller.PreviewMouseWheel += new MouseWheelEventHandler(vacVisualMapView_MouseWheel);
        }

        void vacVisualMapView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl)
                _isZooming = e.IsDown;
        }

        void vacVisualMapView_KeyDown(object sender, KeyEventArgs e)
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