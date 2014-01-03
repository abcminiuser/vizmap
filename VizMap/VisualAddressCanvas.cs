using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace FourWalledCubicle.VizMap
{
    class VisualAddressCanvas : Canvas
    {
        public struct SymbolInfo
        {
            public long Address { get; set; }
            public int Size { get; set; }
        }

        public List<SymbolInfo> Symbols { get; private set; }

        public double Zoom
        {
            get
            {
                return _zoom;
            }

            set
            {
                if (value < 1)
                    value = 1;

                _zoom = value;
                this.InvalidateMeasure();
            }
        }

        private FormattedText _startAddressText;
        private FormattedText _endAddressText;
        private readonly Pen _borderPen = new Pen(Brushes.Black, 1);

        private double _zoom = 1;
        private long _startAddress = 0x0000000;
        private long _endAddress = 0x0007000;

        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(
                availableSize.Width > 0 ? availableSize.Width : 100,
                (_endAddress - _startAddress) / Zoom);
        }

        public VisualAddressCanvas()
        {
            Symbols = new List<SymbolInfo>();

            UpdateStartEndAddressText();
        }

        private void UpdateStartEndAddressText()
        {
            _startAddressText = new FormattedText(
                string.Format("0x{0:x8}", _startAddress),
                System.Globalization.CultureInfo.CurrentCulture,
                System.Windows.FlowDirection.LeftToRight,
                new Typeface(new FontFamily("Consolas"),
                FontStyles.Normal, FontWeights.Normal, new FontStretch()), 12, Brushes.Black);

            _endAddressText = new FormattedText(
                string.Format("0x{0:x8}", _endAddress),
                System.Globalization.CultureInfo.CurrentCulture,
                System.Windows.FlowDirection.LeftToRight,
                new Typeface(new FontFamily("Consolas"),
                FontStyles.Normal, FontWeights.Normal, new FontStretch()), 12, Brushes.Black);
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (this.ActualWidth < _startAddressText.Width)
                return;

            if (this.ActualHeight < 0)
                return;

            dc.DrawText(_startAddressText, new Point(0, 0));
            dc.DrawText(_endAddressText, new Point(0, this.ActualHeight - _endAddressText.Height));

            Rect mapBounds = new Rect(_startAddressText.Width, 0, this.ActualWidth - _startAddressText.Width, this.ActualHeight);
            mapBounds.Inflate(-5, -_startAddressText.Height / 2);
            dc.DrawRectangle(Brushes.White, _borderPen, mapBounds);
        }
    }
}
