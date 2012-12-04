using System;
using System.Collections;
using System.Text;

namespace JFKCommonLibrary.ExtensionMethods
{
    public static class ByteArrayExtensions
    {
        public static bool ByteArrayCompare(this byte[] a1, byte[] a2)
        {
            IStructuralEquatable eqa1 = a1;
            return eqa1.Equals(a2, StructuralComparisons.StructuralEqualityComparer);
        }

        public static string AsHexString(this byte[] value)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (Byte bt in ((System.Byte[])value))
            {
                if (sb.Length > 1) sb.Append(";");
                sb.Append(bt.ToString());
            }
            sb.Append("}");
            return sb.ToString();
        }
    }
}
