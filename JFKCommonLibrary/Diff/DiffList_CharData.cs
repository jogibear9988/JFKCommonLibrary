using System;

namespace JFKCommonLibrary.Diff
{
	public class DiffList_CharData : IDiffList
	{
		private char[] _charList;

		public DiffList_CharData(string charData)
		{
			this._charList = charData.ToCharArray();
		}
		#region IDiffList Members

		public int Count()
		{
			return this._charList.Length;
		}

		public IComparable GetByIndex(int index)
		{
			return this._charList[index];
		}

		#endregion
	}
}