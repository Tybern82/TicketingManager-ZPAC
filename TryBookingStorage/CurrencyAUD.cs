using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TryBookingStorage {

    public class CurrencyAUD {
        private readonly long _cents;

        public CurrencyAUD(long cents) {
            this._cents = cents;
        }

        public CurrencyAUD(int dollars, int cents) : this(dollars * 100 + cents) { }

        public CurrencyAUD(double value) : this ((long)(value * 100)) { }

        public int dollars {
            get { return (int)Math.Abs(_cents / 100); }
        }

        public int cents {
            get { return (int)Math.Abs(_cents % 100); }
        }

        public override String ToString() {
            return "$" + ((_cents < 0) ? "-" : "") + dollars + ((cents != 0) ? ("." + ((cents < 10) ? "0" : "") + cents) : "");
        }

        public override Int32 GetHashCode() {
            return (int)_cents;
        }

        public override Boolean Equals(Object obj) {
            CurrencyAUD val = obj as CurrencyAUD;
            if (val == null) return false;
            return (_cents == val._cents);
        }

        public static CurrencyAUD Parse(string s) {
            string pattern = @"^\$[-]?([0]|([1-9][0-9]*))(\.[0-9]{2}?)?$";
            s = s.Trim();   // clear out whitespace surrounding the string
            Match m = Regex.Match(s.Trim(), pattern);
            if (m.Value == String.Empty) return null;
            // $[0-9]* or $[0-9]*.[0-9][0-9]
            bool isNegative = (Regex.Match(m.Value, "[-]").Value != String.Empty);
            m = Regex.Match(m.Value, "[0-9]+");
            int dollars = int.Parse(m.Value);
            m = m.NextMatch();
            int cents = (m.Value != String.Empty) ? int.Parse(m.Value) : 0;
            int value = dollars * 100 + cents;
            if (isNegative) value = -value;
            return new CurrencyAUD(value);
        }

        public static CurrencyAUD operator +(CurrencyAUD c1, CurrencyAUD c2) {
            return new CurrencyAUD(c1._cents + c2._cents);
        }

        public static CurrencyAUD operator -(CurrencyAUD c1, CurrencyAUD c2) {
            return new CurrencyAUD(c1._cents - c2._cents);
        }

        public static CurrencyAUD operator *(CurrencyAUD c1, long b) {
            return new CurrencyAUD(c1._cents * b);
        }

        public static CurrencyAUD operator /(CurrencyAUD c1, long b) {
            return new CurrencyAUD(c1._cents / b);
        }

        public static CurrencyAUD operator -(CurrencyAUD c) {
            return new CurrencyAUD(-c._cents);
        }

        public static explicit operator CurrencyAUD(string s) {
            CurrencyAUD _result = CurrencyAUD.Parse(s);
            if (_result == null) throw new InvalidCastException("Attempt to convert a non-currency string to a currency value.");
            return _result;
        }

        public static implicit operator string(CurrencyAUD c) {
            return c.ToString();
        }

        public static implicit operator CurrencyAUD(long v) {
            return new CurrencyAUD(v);
        }

        public static implicit operator long(CurrencyAUD c) {
            return c._cents;
        }

        public static implicit operator CurrencyAUD(decimal d) {
            return new CurrencyAUD((long)Math.Round(d * 100));
        }
    }
}
