using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Globalization;

namespace FourWalledCubicle.VizMap
{
    class VisualAddressCanvas : Canvas
    {
        public List<SymbolInfo> Symbols { get; set; }

        public double Zoom
        {
            get
            {
                return _zoom;
            }

            set
            {
                value = Math.Max(value, 1);
                
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

        private const int BYTES_PER_LINE = 64;

        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(
                availableSize.Width > 0 ? availableSize.Width : 100,
                (_endAddress - _startAddress) * 10 / Zoom);
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
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(new FontFamily("Consolas"),
                FontStyles.Normal, FontWeights.Normal, new FontStretch()), 12, Brushes.Black);

            _endAddressText = new FormattedText(
                string.Format("0x{0:x8}", _endAddress),
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(new FontFamily("Consolas"),
                FontStyles.Normal, FontWeights.Normal, new FontStretch()), 12, Brushes.Black);
        }

        protected override void OnRender(DrawingContext dc)
        {
            dc.DrawText(_startAddressText, new Point(0, 0));
            dc.DrawText(_endAddressText, new Point(0, this.ActualHeight - _endAddressText.Height));

            Rect mapBounds = new Rect(
                _startAddressText.Width,
                0,
                this.ActualWidth - _startAddressText.Width,
                this.ActualHeight);

            mapBounds.Inflate(-5, -_startAddressText.Height / 2);
            dc.DrawRectangle(Brushes.White, _borderPen, mapBounds);
            mapBounds.Inflate(-_borderPen.Thickness, -_borderPen.Thickness);

            Rect blockBounds = new Rect(
                mapBounds.Left,
                mapBounds.Top,
                mapBounds.Width / BYTES_PER_LINE,
                mapBounds.Height / ((_endAddress - _startAddress) / BYTES_PER_LINE));
            foreach (SymbolInfo symbol in Symbols)
            {
                long currLine = symbol.StartAddress / BYTES_PER_LINE;
                long currLinePos = symbol.StartAddress % BYTES_PER_LINE;
                long currSizeRem = symbol.Size;

                while (currSizeRem > 0)
                {
                    long currLength = Math.Min(currSizeRem, BYTES_PER_LINE - currLinePos);

                    DrawBlock(dc, blockBounds, currLine, currLinePos, currLength, symbol.Name, Brushes.LightGray);

                    currLine++;
                    currLinePos = 0;
                    currSizeRem -= currLength;
                }
            }
        }

        private void DrawBlock(DrawingContext dc, Rect blockBounds, long line, long start, long width, string name, Brush fill)
        {
            Rect currBlockBounds = new Rect(
                blockBounds.Left + (blockBounds.Width * start),
                blockBounds.Top + (blockBounds.Height * line),
                blockBounds.Width * width,
                blockBounds.Height);

            dc.DrawRectangle(fill, _borderPen, currBlockBounds);

            FormattedText symbolNameText =
                new FormattedText(
                name,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(new FontFamily("Consolas"),
                FontStyles.Normal, FontWeights.Normal, new FontStretch()), currBlockBounds.Height, Brushes.Black);

            symbolNameText.MaxTextWidth = currBlockBounds.Width;
            symbolNameText.Trimming = TextTrimming.CharacterEllipsis;
            dc.DrawText(symbolNameText, currBlockBounds.TopLeft);        
        }
    }
}
