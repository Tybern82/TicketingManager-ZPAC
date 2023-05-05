using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportPrinter {
    public class SeatingMapPrinter : BasePrinter {

        public List<DoorListEntry> doorList { get; set; } = new List<DoorListEntry>();

        public SeatingMapPrinter() {
            DocumentName = "seatingMap";
            DefaultPageSettings.Landscape = true;
            // Since "Toilets" and "Entrance" are written vertically, we need to make sure the height of this area
            // can accomodate them.
            frontAisleHeight = requiredWidth(new string[] { Toilets, Entrance }, headerLineHeight);
        }

        private int frontAisleHeight;

        public static readonly string Title = "Z-PAC Theatre Ticket Sales";
        public static readonly string ForShow = "{0} on {1}";
        public static readonly string Stage = "Stage";
        public static readonly string Aisle = "Aisle";
        public static readonly string Toilets = "Toilets";
        public static readonly string Entrance = "Entrance";
        public static readonly string BlankSpace = "__________";

        private string getShowString() {
            if ((doorList.Count == 0) || (doorList[0] == null)) {
                return String.Format(ForShow, BlankSpace, BlankSpace);
            } else {
                return String.Format(ForShow, doorList[0].eventName, doorList[0].sessionTime.ToString("d-MMM-yyyy h:mm tt"));
            }
        }

        protected override void OnBeginPrint(PrintEventArgs e) {
            base.OnBeginPrint(e);
            PrinterSettings.DefaultPageSettings.Landscape = true;
        }

        protected override void OnPrintPage(PrintPageEventArgs e) {
            base.OnPrintPage(e);

            ISet<Seat> soldSeats = new HashSet<Seat>();
            foreach (DoorListEntry i in doorList) {
                soldSeats.Add(i.seat);
            }

            RectangleF printableArea = PrinterSettings.DefaultPageSettings.PrintableArea;
            int minMargins = (DoorListPrinter.DefaultTitleFont.Height * 2);
            int hardLeft = (int)Math.Round(printableArea.Left, MidpointRounding.AwayFromZero);
            int hardTop = (int)Math.Round(printableArea.Top, MidpointRounding.AwayFromZero);
            int hardRight = (PrinterSettings.DefaultPageSettings.Landscape ? PrinterSettings.DefaultPageSettings.Bounds.Height : PrinterSettings.DefaultPageSettings.Bounds.Width) - (int)Math.Round(printableArea.Right, MidpointRounding.AwayFromZero);
            int hardBottom = (PrinterSettings.DefaultPageSettings.Landscape ? PrinterSettings.DefaultPageSettings.Bounds.Width : PrinterSettings.DefaultPageSettings.Bounds.Height) - (int)Math.Round(printableArea.Bottom, MidpointRounding.AwayFromZero);

            int leftMargin = Math.Max(hardLeft, minMargins);
            int topMargin = Math.Max(hardTop, minMargins);

            int printableWidth = (int)(PrinterSettings.DefaultPageSettings.Landscape ? printableArea.Height : printableArea.Width);
            if (hardLeft < minMargins) printableWidth -= (minMargins - hardLeft);
            if (hardRight < minMargins) printableWidth -= (minMargins - hardRight);
            int printableHeight = (int)(PrinterSettings.DefaultPageSettings.Landscape ? printableArea.Width : printableArea.Height);
            if (hardTop < minMargins) printableHeight -= (minMargins - hardTop);
            if (hardBottom < minMargins) printableHeight -= (minMargins - hardBottom);

            StringFormat fmt = new StringFormat(StringFormatFlags.LineLimit | StringFormatFlags.NoWrap | StringFormatFlags.FitBlackBox);

            Pen blackPen = new Pen(Brushes.Black, 1);
            Pen redPen = new Pen(Brushes.Red, 1);

            int drawBorderAt = 4;
            int boxSize = bodyFont.Height * 2 / 3;
            int bufferDiff = buffer - drawBorderAt;

            Rectangle layoutRect;

            fmt.Alignment = StringAlignment.Center;
            fmt.LineAlignment = StringAlignment.Near;

            layoutRect = new Rectangle(leftMargin, topMargin, printableWidth, headerLineHeight);

            // layoutRect = new RectangleF(new PointF(leftMargin, topMargin), new SizeF(printableWidth, headerLineHeight));
            e.Graphics.DrawString(Title, headerFont, Brushes.Black, layoutRect, fmt);
            int h = headerLineHeight + buffer;
            topMargin += h;
            printableHeight -= h;

            layoutRect = new Rectangle(leftMargin, topMargin, printableWidth, subHeaderLineHeight);
            // layoutRect = new RectangleF(new PointF(leftMargin, topMargin), new SizeF(printableWidth, subHeaderLineHeight));
            e.Graphics.DrawString(getShowString(), subHeaderFont, Brushes.Black, layoutRect, fmt);
            h = subHeaderLineHeight + buffer;
            topMargin += h;
            printableHeight -= h;

            int tableWidth = printableWidth;
            if (tableWidth < printableWidth) leftMargin += (printableWidth - tableWidth) / 2;
            printableWidth = Math.Min(printableWidth, tableWidth);
            int boxWidth = printableWidth / 16;
            printableWidth = boxWidth * 16;

            fmt.Alignment = StringAlignment.Center;
            fmt.LineAlignment = StringAlignment.Center;

            // Draw Stage Area
            Rectangle stageArea = new Rectangle(leftMargin, topMargin, printableWidth, headerLineHeight + buffer + buffer);
            e.Graphics.DrawRectangle(blackPen, stageArea);
            // e.Graphics.DrawLine(blackPen, leftMargin, topMargin, leftMargin + printableWidth, topMargin);
            // topMargin += buffer - drawBorderAt;
            e.Graphics.DrawString(Stage, headerFont, Brushes.Black, stageArea, fmt);
            topMargin += headerLineHeight + buffer + buffer;
            printableHeight -= headerLineHeight + buffer + buffer;

            // Draw Front Aisle Area
            Rectangle frontAisle = new Rectangle(leftMargin, topMargin, printableWidth, frontAisleHeight + buffer + buffer);
            e.Graphics.DrawRectangle(blackPen, frontAisle);
            e.Graphics.DrawString(Aisle, headerFont, Brushes.Black, frontAisle, fmt);
            fmt.LineAlignment = StringAlignment.Near;
            fmt.FormatFlags |= StringFormatFlags.DirectionVertical;
            e.Graphics.DrawString(Toilets, subHeaderFont, Brushes.Black, frontAisle, fmt);
            fmt.LineAlignment = StringAlignment.Far;
            e.Graphics.DrawString(Entrance, subHeaderFont, Brushes.Black, frontAisle, fmt);
            topMargin += frontAisleHeight + buffer + buffer;
            printableHeight -= frontAisleHeight + buffer + buffer;

            int boxHeight = printableHeight / 9;

            // Draw Centre Aisle Area
            fmt.Alignment = StringAlignment.Center;
            fmt.LineAlignment = StringAlignment.Center;
            Rectangle centreAisle = getArea(boxHeight, boxWidth, topMargin, leftMargin, 0, 7, 2, 8);
            e.Graphics.DrawRectangle(blackPen, centreAisle);
            e.Graphics.DrawString(Aisle, headerFont, Brushes.Black, centreAisle, fmt);
            fmt.FormatFlags &= ~StringFormatFlags.DirectionVertical;

            // Draw Seats
            Array seats = Enum.GetValues(typeof(Seat));
            foreach (Seat s in seats) {
                if (s != Seat.XTD) {
                    Rectangle seatArea = getArea(boxHeight, boxWidth, topMargin, leftMargin, getRow(s), getColumn(s), 1, 1);
                    e.Graphics.DrawRectangle(blackPen, seatArea);
                    if (soldSeats.Contains(s)) {
                        // need to mark as sold
                        e.Graphics.FillRectangle(Brushes.LightGray, addMargin(seatArea, buffer));
                        if (DoorListReportPrinter.MarkThroughSeats) {
                            // if (Settings.ActiveSettings.MarkOutSoldSeats) {
                            e.Graphics.DrawLine(redPen, seatArea.Left, seatArea.Top, seatArea.Right, seatArea.Bottom);
                            e.Graphics.DrawLine(redPen, seatArea.Right, seatArea.Top, seatArea.Left, seatArea.Bottom);
                        }
                    }
                    e.Graphics.DrawString(s.ToString(), bodyFont, Brushes.Black, seatArea, fmt);
                }
            }
        }

        private int getRow(Seat s) {
            switch (s) {
                case Seat.A1:
                case Seat.A2:
                case Seat.A3:
                case Seat.A4:
                case Seat.A5:
                case Seat.A6:
                case Seat.A7:
                case Seat.A8:
                case Seat.A9:
                case Seat.A10:
                case Seat.A11:
                case Seat.A12:
                case Seat.A13:
                case Seat.A14:
                    return 0;
                case Seat.B1:
                case Seat.B2:
                case Seat.B3:
                case Seat.B4:
                case Seat.B5:
                case Seat.B6:
                case Seat.B7:
                case Seat.B8:
                case Seat.B9:
                case Seat.B10:
                case Seat.B11:
                case Seat.B12:
                case Seat.B13:
                case Seat.B14:
                    return 1;
                case Seat.C1:
                case Seat.C2:
                case Seat.C3:
                case Seat.C4:
                case Seat.C5:
                case Seat.C6:
                case Seat.C7:
                case Seat.C8:
                case Seat.C9:
                case Seat.C10:
                case Seat.C11:
                case Seat.C12:
                case Seat.C13:
                case Seat.C14:
                    return 2;
                case Seat.D1:
                case Seat.D2:
                case Seat.D3:
                case Seat.D4:
                case Seat.D5:
                case Seat.D6:
                case Seat.D7:
                case Seat.D8:
                case Seat.D9:
                case Seat.D10:
                case Seat.D11:
                case Seat.D12:
                case Seat.D13:
                case Seat.D14:
                    return 3;
                case Seat.E1:
                case Seat.E2:
                case Seat.E3:
                case Seat.E4:
                case Seat.E5:
                case Seat.E6:
                case Seat.E7:
                case Seat.E8:
                case Seat.E9:
                case Seat.E10:
                case Seat.E11:
                case Seat.E12:
                case Seat.E13:
                case Seat.E14:
                    return 4;
                case Seat.F1:
                case Seat.F2:
                case Seat.F3:
                case Seat.F4:
                case Seat.F5:
                case Seat.F6:
                case Seat.F7:
                case Seat.F8:
                case Seat.F9:
                case Seat.F10:
                case Seat.F11:
                case Seat.F12:
                case Seat.F13:
                case Seat.F14:
                    return 5;
                case Seat.G1:
                case Seat.G2:
                case Seat.G3:
                case Seat.G4:
                case Seat.G5:
                case Seat.G6:
                case Seat.G7:
                case Seat.G8:
                case Seat.G9:
                case Seat.G10:
                case Seat.G11:
                case Seat.G12:
                case Seat.G13:
                case Seat.G14:
                    return 6;
                case Seat.H1:
                case Seat.H2:
                case Seat.H3:
                case Seat.H4:
                case Seat.H5:
                case Seat.H6:
                case Seat.H7:
                case Seat.H8:
                case Seat.H9:
                case Seat.H10:
                case Seat.H11:
                case Seat.H12:
                case Seat.H13:
                case Seat.H14:
                    return 7;
                case Seat.I1:
                case Seat.I2:
                case Seat.I3:
                case Seat.I4:
                case Seat.I5:
                case Seat.I6:
                case Seat.I7:
                case Seat.I8:
                case Seat.I9:
                case Seat.I10:
                case Seat.I11:
                case Seat.I12:
                case Seat.I13:
                case Seat.I14:
                case Seat.J1:
                case Seat.J2:
                    return 8;
                default:
                    return -1;
            }
        }

        private int getColumn(Seat s) {
            switch (s) {
                case Seat.A14:
                case Seat.B14:
                case Seat.C14:
                case Seat.D14:
                case Seat.E14:
                case Seat.F14:
                case Seat.G14:
                case Seat.H14:
                case Seat.I14:
                    return 0;
                case Seat.A13:
                case Seat.B13:
                case Seat.C13:
                case Seat.D13:
                case Seat.E13:
                case Seat.F13:
                case Seat.G13:
                case Seat.H13:
                case Seat.I13:
                    return 1;
                case Seat.A12:
                case Seat.B12:
                case Seat.C12:
                case Seat.D12:
                case Seat.E12:
                case Seat.F12:
                case Seat.G12:
                case Seat.H12:
                case Seat.I12:
                    return 2;
                case Seat.A11:
                case Seat.B11:
                case Seat.C11:
                case Seat.D11:
                case Seat.E11:
                case Seat.F11:
                case Seat.G11:
                case Seat.H11:
                case Seat.I11:
                    return 3;
                case Seat.A10:
                case Seat.B10:
                case Seat.C10:
                case Seat.D10:
                case Seat.E10:
                case Seat.F10:
                case Seat.G10:
                case Seat.H10:
                case Seat.I10:
                    return 4;
                case Seat.A9:
                case Seat.B9:
                case Seat.C9:
                case Seat.D9:
                case Seat.E9:
                case Seat.F9:
                case Seat.G9:
                case Seat.H9:
                case Seat.I9:
                    return 5;
                case Seat.A8:
                case Seat.B8:
                case Seat.C8:
                case Seat.D8:
                case Seat.E8:
                case Seat.F8:
                case Seat.G8:
                case Seat.H8:
                case Seat.I8:
                    return 6;
                case Seat.J1:
                    return 7;
                case Seat.J2:
                    return 8;
                case Seat.A7:
                case Seat.B7:
                case Seat.C7:
                case Seat.D7:
                case Seat.E7:
                case Seat.F7:
                case Seat.G7:
                case Seat.H7:
                case Seat.I7:
                    return 9;
                case Seat.A6:
                case Seat.B6:
                case Seat.C6:
                case Seat.D6:
                case Seat.E6:
                case Seat.F6:
                case Seat.G6:
                case Seat.H6:
                case Seat.I6:
                    return 10;
                case Seat.A5:
                case Seat.B5:
                case Seat.C5:
                case Seat.D5:
                case Seat.E5:
                case Seat.F5:
                case Seat.G5:
                case Seat.H5:
                case Seat.I5:
                    return 11;
                case Seat.A4:
                case Seat.B4:
                case Seat.C4:
                case Seat.D4:
                case Seat.E4:
                case Seat.F4:
                case Seat.G4:
                case Seat.H4:
                case Seat.I4:
                    return 12;
                case Seat.A3:
                case Seat.B3:
                case Seat.C3:
                case Seat.D3:
                case Seat.E3:
                case Seat.F3:
                case Seat.G3:
                case Seat.H3:
                case Seat.I3:
                    return 13;
                case Seat.A2:
                case Seat.B2:
                case Seat.C2:
                case Seat.D2:
                case Seat.E2:
                case Seat.F2:
                case Seat.G2:
                case Seat.H2:
                case Seat.I2:
                    return 14;
                case Seat.A1:
                case Seat.B1:
                case Seat.C1:
                case Seat.D1:
                case Seat.E1:
                case Seat.F1:
                case Seat.G1:
                case Seat.H1:
                case Seat.I1:
                    return 15;
                default:
                    return -1;
            }
        }
    }
}
