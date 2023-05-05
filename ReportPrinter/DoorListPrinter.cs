using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReportPrinter {
    public class DoorListPrinter : BasePrinter {

        public DoorListPrinter() {
            DocumentName = "doorList";
            OnSubHeaderFontChange += subHeaderFontChange;
        }

        private void subHeaderFontChange(object sender, FontChangeEventArgs e) {
            if (e.previousFont != e.currentFont) {
                // Mark the headers as needing recalculation
                _NameHeaderWidth = -1;
                _PhoneHeaderWidth = -1;
                _TicketHeaderWidth = -1;
                _PromoHeaderWidth = -1;
                _SeatHeaderWidth = -1;
            }
        }

        private int _NameHeaderWidth = -1;
        private int NameHeaderWidth {
            get {
                if (_NameHeaderWidth == -1) _NameHeaderWidth = TextRenderer.MeasureText(this.PrinterSettings.CreateMeasurementGraphics(), NameHeader, subHeaderFont, new Size(printableWidth, subHeaderLineHeight)).Width;
                return _NameHeaderWidth;
            }
        }

        private int _PhoneHeaderWidth = -1;
        private int PhoneHeaderWidth {
            get {
                if (_PhoneHeaderWidth == -1) _PhoneHeaderWidth = TextRenderer.MeasureText(this.PrinterSettings.CreateMeasurementGraphics(), PhoneHeader, subHeaderFont, new Size(printableWidth, subHeaderLineHeight)).Width;
                return _PhoneHeaderWidth;
            }
        }

        private int _TicketHeaderWidth = -1;
        private int TicketHeaderWidth {
            get {
                if (_TicketHeaderWidth == -1) _TicketHeaderWidth = TextRenderer.MeasureText(this.PrinterSettings.CreateMeasurementGraphics(), TicketHeader, subHeaderFont, new Size(printableWidth, subHeaderLineHeight)).Width;
                return _TicketHeaderWidth;
            }
        }

        private int _PromoHeaderWidth = -1;
        private int PromoHeaderWidth {
            get {
                if (_PromoHeaderWidth == -1) _PromoHeaderWidth = TextRenderer.MeasureText(this.PrinterSettings.CreateMeasurementGraphics(), PromoHeader, subHeaderFont, new Size(printableWidth, subHeaderLineHeight)).Width;
                return _PromoHeaderWidth;
            }
        }

        private int _SeatHeaderWidth = -1;
        private int SeatHeaderWidth {
            get {
                if (_SeatHeaderWidth == -1) _SeatHeaderWidth = TextRenderer.MeasureText(this.PrinterSettings.CreateMeasurementGraphics(), SeatHeader, subHeaderFont, new Size(printableWidth, subHeaderLineHeight)).Width;
                return _SeatHeaderWidth;
            }
        }

        private static readonly string NameHeader = "Name";
        private static readonly string PhoneHeader = "Phone";
        private static readonly string TicketHeader = "Ticket";
        private static readonly string PromoHeader = "Promo";
        private static readonly string SeatHeader = "Seat";

        public string listTitle { get; set; }

        private List<DoorListEntry> _doorList = new List<DoorListEntry>();
        public List<DoorListEntry> doorList {
            get { return _doorList; }
            set {
                _doorList = value;
                doorListSizes = null;
            }
        }

        static string FreeDrinkText = "FD";

        public DoorListEntrySizes doorListSizes { get; set; } = null;

        // int firstNameWidth { get; set; } = 0;
        // int lastNameWidth { get; set; } = 0;
        // int contactNumberWidth { get; set; } = 0;
        // int ticketTypeWidth { get; set; } = 0;
        // int ticketPriceWidth { get; set; } = 0;
        // int promoCodeWidth { get; set; } = 0;
        // int seatWidth { get; set; } = 0;
        // int freeDrinkWidth { get; set; } = 0;

        private int currentRow;
        private int currentPage;

        private void loadSizes() {
            doorListSizes = new DoorListEntrySizes();
            EnumerationGenerator gen = new EnumerationGenerator(doorList);
            doorListSizes.firstNameWidth = requiredWidth(gen.firstNames, 0);
            int minWidth = Math.Max(0, NameHeaderWidth - doorListSizes.firstNameWidth - buffer - buffer);
            doorListSizes.lastNameWidth = requiredWidth(gen.lastNames, minWidth);

            minWidth = Math.Max(0, PhoneHeaderWidth - buffer);
            doorListSizes.contactNumberWidth = requiredWidth(gen.contactNumbers, minWidth);

            doorListSizes.ticketPriceWidth = requiredWidth(gen.ticketPrices, 0);
            minWidth = Math.Max(0, TicketHeaderWidth - buffer - buffer - doorListSizes.ticketPriceWidth);
            doorListSizes.ticketTypeWidth = requiredWidth(gen.ticketTypes, minWidth);

            minWidth = Math.Max(0, PromoHeaderWidth - buffer);
            doorListSizes.promoCodeWidth = requiredWidth(gen.promoCodes, minWidth);

            minWidth = Math.Max(0, SeatHeaderWidth - buffer);
            doorListSizes.seatWidth = requiredWidth(gen.seats, minWidth);

            doorListSizes.freeDrinkWidth = TextRenderer.MeasureText(FreeDrinkText, bodyFont, new Size(printableWidth, lineHeight)).Width;
        }

        protected override void OnBeginPrint(System.Drawing.Printing.PrintEventArgs e) {
            base.OnBeginPrint(e);

            if (doorListSizes == null) loadSizes();

            currentRow = 0;     // reset to the first row
            currentPage = 0;    // reset to the first page
        }

        protected override void OnPrintPage(System.Drawing.Printing.PrintPageEventArgs e) {
            base.OnPrintPage(e);

            /*
            int leftMargin = this.leftMargin;
            int topMargin = this.topMargin;
            int printableWidth = this.printableWidth;
            int printableHeight = this.printableHeight;
            */

            RectangleF printableArea = DefaultPageSettings.PrintableArea;
            int minMargins = (DefaultTitleFont.Height * 2);
            int hardLeft = (int)Math.Round(printableArea.Left, MidpointRounding.AwayFromZero);
            int hardTop = (int)Math.Round(printableArea.Top, MidpointRounding.AwayFromZero);
            int hardRight = DefaultPageSettings.Bounds.Width - (int)Math.Round(printableArea.Right, MidpointRounding.AwayFromZero);
            int hardBottom = DefaultPageSettings.Bounds.Height - (int)Math.Round(printableArea.Bottom, MidpointRounding.AwayFromZero);

            int leftMargin = Math.Max(hardLeft, minMargins);
            int topMargin = Math.Max(hardTop, minMargins);

            int printableWidth = (int)printableArea.Width;
            if (hardLeft < minMargins) printableWidth -= (minMargins - hardLeft);
            if (hardRight < minMargins) printableWidth -= (minMargins - hardRight);

            int printableHeight = (int)printableArea.Height;
            if (hardTop < minMargins) printableHeight -= (minMargins - hardTop);
            if (hardBottom < minMargins) printableHeight -= (minMargins - hardBottom);

            int drawBorderAt = 4;
            int boxSize = bodyFont.Height * 2 / 3;
            int bufferDiff = buffer - drawBorderAt;

            string sessionTitle = (doorList.Count() > 0) ? doorList[0].eventName : "";
            string sessionTime = (doorList.Count() > 0) ? doorList[0].sessionTime.ToString("d-MMM-yyyy h:mm tt") : "";

            StringFormat fmt = new StringFormat(StringFormatFlags.LineLimit | StringFormatFlags.NoWrap | StringFormatFlags.FitBlackBox);

            Pen blackPen = new Pen(Brushes.Black, 1);

            RectangleF layoutRect;

            fmt.Alignment = StringAlignment.Center;
            fmt.LineAlignment = StringAlignment.Near;

            layoutRect = new RectangleF(new PointF(leftMargin, topMargin), new SizeF(printableWidth, headerLineHeight));
            e.Graphics.DrawString(sessionTitle, headerFont, Brushes.Black, layoutRect, fmt);
            int h = headerLineHeight + buffer;
            topMargin += h;
            printableHeight -= h;

            layoutRect = new RectangleF(new PointF(leftMargin, topMargin), new SizeF(printableWidth, subHeaderLineHeight));
            e.Graphics.DrawString(sessionTime, subHeaderFont, Brushes.Black, layoutRect, fmt);
            h = subHeaderLineHeight + buffer;
            topMargin += h;
            printableHeight -= h;


            layoutRect = new RectangleF(new PointF(leftMargin, topMargin), new SizeF(printableWidth, subHeaderLineHeight));
            e.Graphics.DrawString(listTitle, subHeaderFont, Brushes.Black, layoutRect, fmt);
            h = subHeaderLineHeight + buffer;
            topMargin += h;
            printableHeight -= h;


            topMargin += buffer;
            printableHeight -= buffer;

            fmt.LineAlignment = StringAlignment.Far;
            layoutRect = new RectangleF(new PointF(leftMargin, topMargin), new SizeF(printableWidth, printableHeight));
            // fmt.Alignment = StringAlignment.Near;
            // e.Graphics.DrawString(sessionTime, subHeaderFont, Brushes.Black, layoutRect, fmt);
            fmt.Alignment = StringAlignment.Far;
            e.Graphics.DrawString("Page " + (currentPage + 1), subHeaderFont, Brushes.Black, layoutRect, fmt);
            printableHeight -= subHeaderLineHeight + buffer;

            int tableWidth = doorListSizes.tableWidth + boxSize + (buffer * 11);
            if (tableWidth < printableWidth) leftMargin += (printableWidth - tableWidth) / 2;
            printableWidth = Math.Min(printableWidth, tableWidth);

            e.Graphics.DrawLine(blackPen, leftMargin, topMargin, leftMargin + printableWidth, topMargin);
            topMargin += buffer - drawBorderAt;
            printableHeight -= buffer;

            // Print Table Headers

            fmt.LineAlignment = StringAlignment.Center;
            e.Graphics.DrawLine(blackPen, leftMargin, topMargin - bufferDiff, leftMargin, topMargin + subHeaderLineHeight + drawBorderAt);
            fmt.Alignment = StringAlignment.Center;
            RectangleF box = new RectangleF(new PointF(leftMargin, topMargin), new SizeF(doorListSizes.firstNameWidth + buffer + doorListSizes.lastNameWidth + buffer, subHeaderLineHeight + buffer));
            e.Graphics.DrawString(NameHeader, subHeaderFont, Brushes.Black, box, fmt);
            float linePos = box.Left + doorListSizes.firstNameWidth + doorListSizes.lastNameWidth + buffer + buffer + drawBorderAt;
            e.Graphics.DrawLine(blackPen, linePos, topMargin - bufferDiff, linePos, topMargin + subHeaderLineHeight + drawBorderAt);

            box = new RectangleF(new PointF(linePos + bufferDiff, topMargin), new SizeF(doorListSizes.contactNumberWidth + buffer, subHeaderLineHeight + buffer));
            e.Graphics.DrawString(PhoneHeader, subHeaderFont, Brushes.Black, box, fmt);
            linePos = box.Left + doorListSizes.contactNumberWidth + drawBorderAt;
            e.Graphics.DrawLine(blackPen, linePos, topMargin - bufferDiff, linePos, topMargin + subHeaderLineHeight + drawBorderAt);

            box = new RectangleF(new PointF(linePos + bufferDiff, topMargin), new SizeF(doorListSizes.ticketTypeWidth + buffer + doorListSizes.ticketPriceWidth + buffer, subHeaderLineHeight + buffer));
            e.Graphics.DrawString(TicketHeader, subHeaderFont, Brushes.Black, box, fmt);
            linePos = box.Left + doorListSizes.ticketTypeWidth + buffer + doorListSizes.ticketPriceWidth + drawBorderAt;
            e.Graphics.DrawLine(blackPen, linePos, topMargin - bufferDiff, linePos, topMargin + subHeaderLineHeight + drawBorderAt);

            box = new RectangleF(new PointF(linePos + bufferDiff, topMargin), new SizeF(doorListSizes.promoCodeWidth + buffer, subHeaderLineHeight + buffer));
            e.Graphics.DrawString(PromoHeader, subHeaderFont, Brushes.Black, box, fmt);
            linePos = box.Left + doorListSizes.promoCodeWidth + drawBorderAt;
            e.Graphics.DrawLine(blackPen, linePos, topMargin - bufferDiff, linePos, topMargin + subHeaderLineHeight + drawBorderAt);

            box = new RectangleF(new PointF(linePos + bufferDiff, topMargin), new SizeF(doorListSizes.seatWidth + buffer, subHeaderLineHeight + buffer));
            e.Graphics.DrawString(SeatHeader, subHeaderFont, Brushes.Black, box, fmt);
            linePos = box.Left + doorListSizes.seatWidth + drawBorderAt;
            e.Graphics.DrawLine(blackPen, linePos, topMargin - bufferDiff, linePos, topMargin + subHeaderLineHeight + drawBorderAt);

            linePos = linePos + bufferDiff + doorListSizes.freeDrinkWidth + buffer + buffer + buffer + boxSize;
            e.Graphics.DrawLine(blackPen, linePos, topMargin - bufferDiff, linePos, topMargin + subHeaderLineHeight + drawBorderAt);

            e.Graphics.DrawLine(blackPen, leftMargin, topMargin + subHeaderLineHeight + drawBorderAt, leftMargin + printableWidth, topMargin + subHeaderLineHeight + drawBorderAt);

            topMargin += subHeaderLineHeight + buffer;
            printableHeight -= subHeaderLineHeight + buffer;

            while ((currentRow < doorList.Count()) && (printableHeight > (lineHeight + buffer))) {
                DoorListEntry row = doorList[currentRow];

                fmt.LineAlignment = StringAlignment.Center;

                e.Graphics.DrawLine(blackPen, leftMargin, topMargin - bufferDiff, leftMargin, topMargin + lineHeight + drawBorderAt);

                fmt.Alignment = StringAlignment.Near;
                box = new RectangleF(new PointF(leftMargin + buffer, topMargin), new SizeF(doorListSizes.firstNameWidth + buffer, lineHeight + buffer));
                e.Graphics.DrawString(row.firstName, bodyFont, Brushes.Black, box, fmt);
                e.Graphics.DrawLine(blackPen, box.Left + doorListSizes.firstNameWidth + drawBorderAt, topMargin - bufferDiff, box.Left + doorListSizes.firstNameWidth + drawBorderAt, topMargin + lineHeight + drawBorderAt);

                fmt.Alignment = StringAlignment.Near;
                box = new RectangleF(new PointF(box.Left + doorListSizes.firstNameWidth + buffer, topMargin), new SizeF(doorListSizes.lastNameWidth + buffer, lineHeight + buffer));
                e.Graphics.DrawString(row.lastName, bodyFont, Brushes.Black, box, fmt);
                e.Graphics.DrawLine(blackPen, box.Left + doorListSizes.lastNameWidth + drawBorderAt, topMargin - bufferDiff, box.Left + doorListSizes.lastNameWidth + drawBorderAt, topMargin + lineHeight + drawBorderAt);

                fmt.Alignment = StringAlignment.Center;
                box = new RectangleF(new PointF(box.Left + doorListSizes.lastNameWidth + buffer, topMargin), new SizeF(doorListSizes.contactNumberWidth + buffer, lineHeight + buffer));
                e.Graphics.DrawString(row.contactNumber, bodyFont, Brushes.Black, box, fmt);
                e.Graphics.DrawLine(blackPen, box.Left + doorListSizes.contactNumberWidth + drawBorderAt, topMargin - bufferDiff, box.Left + doorListSizes.contactNumberWidth + drawBorderAt, topMargin + lineHeight + drawBorderAt);

                fmt.Alignment = StringAlignment.Center;
                box = new RectangleF(new PointF(box.Left + doorListSizes.contactNumberWidth + buffer, topMargin), new SizeF(doorListSizes.ticketTypeWidth + buffer, lineHeight + buffer));
                e.Graphics.DrawString(TicketTypeHelper.getTicketTypeName(row.ticketType), bodyFont, Brushes.Black, box, fmt);
                e.Graphics.DrawLine(blackPen, box.Left + doorListSizes.ticketTypeWidth + drawBorderAt, topMargin - bufferDiff, box.Left + doorListSizes.ticketTypeWidth + drawBorderAt, topMargin + lineHeight + drawBorderAt);

                fmt.Alignment = StringAlignment.Center;
                box = new RectangleF(new PointF(box.Left + doorListSizes.ticketTypeWidth + buffer, topMargin), new SizeF(doorListSizes.ticketPriceWidth + buffer, lineHeight + buffer));
                e.Graphics.DrawString(row.ticketPrice, bodyFont, Brushes.Black, box, fmt);
                e.Graphics.DrawLine(blackPen, box.Left + doorListSizes.ticketPriceWidth + drawBorderAt, topMargin - bufferDiff, box.Left + doorListSizes.ticketPriceWidth + drawBorderAt, topMargin + lineHeight + drawBorderAt);

                fmt.Alignment = StringAlignment.Center;
                box = new RectangleF(new PointF(box.Left + doorListSizes.ticketPriceWidth + buffer, topMargin), new SizeF(doorListSizes.promoCodeWidth + buffer, lineHeight + buffer));
                e.Graphics.DrawString(row.promoCode.promoCode, bodyFont, Brushes.Black, box, fmt);
                e.Graphics.DrawLine(blackPen, box.Left + doorListSizes.promoCodeWidth + drawBorderAt, topMargin - bufferDiff, box.Left + doorListSizes.promoCodeWidth + drawBorderAt, topMargin + lineHeight + drawBorderAt);

                fmt.Alignment = StringAlignment.Center;
                box = new RectangleF(new PointF(box.Left + doorListSizes.promoCodeWidth + buffer, topMargin), new SizeF(doorListSizes.seatWidth + buffer, lineHeight + buffer));
                e.Graphics.DrawString(row.seat.ToString(), bodyFont, Brushes.Black, box, fmt);
                e.Graphics.DrawLine(blackPen, box.Left + doorListSizes.seatWidth + drawBorderAt, topMargin - bufferDiff, box.Left + doorListSizes.seatWidth + drawBorderAt, topMargin + lineHeight + drawBorderAt);

                fmt.Alignment = StringAlignment.Center;
                box = new RectangleF(new PointF(box.Left + doorListSizes.seatWidth + buffer, topMargin), new SizeF(doorListSizes.freeDrinkWidth + buffer, lineHeight + buffer));
                if (row.promoCode.hasFreeDrink) e.Graphics.DrawString(FreeDrinkText, bodyFont, Brushes.Black, box, fmt);
                e.Graphics.DrawLine(blackPen, box.Left + doorListSizes.freeDrinkWidth + drawBorderAt, topMargin - bufferDiff, box.Left + doorListSizes.freeDrinkWidth + drawBorderAt, topMargin + lineHeight + drawBorderAt);

                float leftPosition = box.Left + doorListSizes.freeDrinkWidth + buffer + buffer;
                e.Graphics.DrawRectangle(blackPen, leftPosition, topMargin + ((lineHeight + buffer - boxSize) / 2), boxSize, boxSize);
                e.Graphics.DrawLine(blackPen, leftPosition + buffer + boxSize, topMargin - bufferDiff, leftPosition + buffer + boxSize, topMargin + lineHeight + drawBorderAt);

                e.Graphics.DrawLine(blackPen, leftMargin, topMargin + lineHeight + drawBorderAt, leftMargin + printableWidth, topMargin + lineHeight + drawBorderAt);

                topMargin += lineHeight + buffer;
                printableHeight -= lineHeight + buffer;
                currentRow++;
            }

            currentPage++;
            e.HasMorePages = (currentRow < doorList.Count);
        }
    }

    class EnumerationGenerator {
        private List<DoorListEntry> doorList;

        public EnumerationGenerator(List<DoorListEntry> dList) {
            this.doorList = dList;
        }

        public IEnumerable<string> firstNames { get { foreach (DoorListEntry d in doorList) yield return d.firstName; } }
        public IEnumerable<string> lastNames { get { foreach (DoorListEntry d in doorList) yield return d.lastName; } }
        public IEnumerable<string> contactNumbers { get { foreach (DoorListEntry d in doorList) yield return d.contactNumber; } }
        public IEnumerable<string> ticketTypes { get { foreach (DoorListEntry d in doorList) yield return TicketTypeHelper.getTicketTypeName(d.ticketType); } }
        public IEnumerable<string> ticketPrices { get { foreach (DoorListEntry d in doorList) yield return d.ticketPrice.ToString(); } }
        public IEnumerable<string> promoCodes { get { foreach (DoorListEntry d in doorList) yield return d.promoCode.promoCode; } } // TODO: promoName?
        public IEnumerable<string> seats { get { foreach (DoorListEntry d in doorList) yield return d.seat.ToString(); } }
    }
}
