using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportPrinter {
    public abstract class BasePrinter : System.Drawing.Printing.PrintDocument {

        protected int buffer = 5;

        public static Font DefaultTitleFont = new Font(FontFamily.GenericSerif, 14);
        public static Font DefaultSubTitleFont = new Font(FontFamily.GenericSerif, 12);
        public static Font DefaultTextFont = new Font(FontFamily.GenericSansSerif, 10);

        protected int marginlessWidth {
            get {
                return (int)DefaultPageSettings.PrintableArea.Width;
            }
        }

        protected int marginlessHeight {
            get {
                return (int)DefaultPageSettings.PrintableArea.Height;
            }
        }

        protected int printableWidth {
            get {
                return (DefaultPageSettings.Landscape ? _downPage() : _acrossPage());
            }
        }

        protected int printableHeight {
            get {
                return (DefaultPageSettings.Landscape ? _acrossPage() : _downPage());
            }
        }

        protected int topMargin {
            get {
                return (DefaultPageSettings.Landscape ? DefaultPageSettings.Margins.Left : DefaultPageSettings.Margins.Top);
            }
        }

        protected int bottomMargin {
            get {
                return (DefaultPageSettings.Landscape ? DefaultPageSettings.Margins.Right : DefaultPageSettings.Margins.Bottom);
            }
        }

        protected int leftMargin {
            get {
                return (DefaultPageSettings.Landscape ? DefaultPageSettings.Margins.Bottom : DefaultPageSettings.Margins.Left);
            }
        }

        protected int rightMargin {
            get {
                return (DefaultPageSettings.Landscape ? DefaultPageSettings.Margins.Top : DefaultPageSettings.Margins.Right);
            }
        }

        protected int lineHeight { get { return bodyFont.Height; } }
        protected int headerLineHeight { get { return headerFont.Height; } }
        protected int subHeaderLineHeight { get { return subHeaderFont.Height; } }

        private Font _bodyFont = DefaultTextFont;
        public Font bodyFont {
            get { return _bodyFont; }
            set {
                if (OnBodyFontChange != null) OnBodyFontChange.Invoke(this, new FontChangeEventArgs(_bodyFont, value));
                _bodyFont = value;
            }
        }

        private Font _headerFont = DefaultTitleFont;
        public Font headerFont {
            get { return _headerFont; }
            set {
                if (OnHeaderFontChange != null) OnHeaderFontChange.Invoke(this, new FontChangeEventArgs(_headerFont, value));
                _headerFont = value;
            }
        }

        private Font _subHeaderFont = DefaultSubTitleFont;
        public Font subHeaderFont {
            get { return _subHeaderFont; }
            set {
                if (OnSubHeaderFontChange != null) OnSubHeaderFontChange.Invoke(this, new FontChangeEventArgs(_subHeaderFont, value));
                _subHeaderFont = value;
            }
        }

        protected delegate void OnFontChange(object sender, FontChangeEventArgs e);

        protected event OnFontChange OnBodyFontChange;
        protected event OnFontChange OnHeaderFontChange;
        protected event OnFontChange OnSubHeaderFontChange;

        protected class FontChangeEventArgs : EventArgs {
            public Font previousFont { get; set; }
            public Font currentFont { get; set; }

            public FontChangeEventArgs(Font previous, Font nFont) {
                this.previousFont = previous;
                this.currentFont = nFont;
            }
        }

        /// <summary>
        /// Determines the required width to display the list of items under the current body font.
        /// </summary>
        /// <param name="g">Graphics context to use for calculating the widths</param>
        /// <param name="items">List of items being displayed</param>
        /// <returns>Minimum width required for this list to fully display, maximized by the printable width of the document</returns>
        public int requiredWidth(IEnumerable<string> items, int minWidth) {
            int width = minWidth;
            StringFormat fmt = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.LineLimit);
            HashSet<string> uniqueItems = new HashSet<string>(items);
            Graphics g = PrinterSettings.CreateMeasurementGraphics();
            foreach (string item in uniqueItems) {
                SizeF sz = g.MeasureString(item, bodyFont, new Size(printableWidth, lineHeight));
                width = Math.Max(width, (int)Math.Round(sz.Width, MidpointRounding.AwayFromZero));
                // Size sz = TextRenderer.MeasureText(g, item, bodyFont, new Size(printableWidth, lineHeight));
                // width = Math.Max(width, sz.Width);
            }
            return width;
        }


        private int _acrossPage() {
            return DefaultPageSettings.PaperSize.Width - DefaultPageSettings.Margins.Left - DefaultPageSettings.Margins.Right;
        }

        private int _downPage() {
            return DefaultPageSettings.PaperSize.Height - DefaultPageSettings.Margins.Top - DefaultPageSettings.Margins.Bottom;
        }

        protected Rectangle addMargin(Rectangle r, int margin) {
            return new Rectangle(r.Left + margin, r.Top + margin, r.Width - (margin * 2), r.Height - (margin * 2));
        }

        /// <summary>
        /// Helper method for table construction. 
        /// </summary>
        /// <param name="boxHeight">Height of each cell in the table</param>
        /// <param name="boxWidth">Width of each cell in the table</param>
        /// <param name="topMargin">Top position of the table</param>
        /// <param name="leftMargin">Left position of the table</param>
        /// <param name="row">Row in the table for top left of area</param>
        /// <param name="column">Column in the table for top left of area</param>
        /// <param name="width">Number of columns area spans</param>
        /// <param name="height">Number of rows area spans</param>
        /// <returns>Graphics rectangle defining an area coverering these cells</returns>
        protected Rectangle getArea(int boxHeight, int boxWidth, int topMargin, int leftMargin, int row, int column, int width, int height) {
            return new Rectangle(leftMargin + (column * boxWidth), topMargin + (row * boxHeight), boxWidth * width, boxHeight * height);
        }
    }
}
