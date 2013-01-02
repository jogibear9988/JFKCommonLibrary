using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JFKCommonLibrary.Diff
{
    public class TextLine : IComparable
    {
        public string Line;
        public int _hash;

        public TextLine(string str)
        {
            this.Line = str.Replace("\t", "    ");
            this._hash = str.GetHashCode();
        }
        #region IComparable Members

        public int CompareTo(object obj)
        {
            return this._hash.CompareTo(((TextLine)obj)._hash);
        }

        #endregion
    }
}
