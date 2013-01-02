using System;
using System.IO;
using System.Collections;

namespace JFKCommonLibrary.Diff
{
	public class DiffList_TextFile : IDiffList
	{
        private const int MaxLineLength = 4096;
		private ArrayList _lines;

		public DiffList_TextFile(string fileName)
		{
			this._lines = new ArrayList();
			using (StreamReader sr = new StreamReader(fileName)) 
			{
				String line;
				// Read and display lines from the file until the end of 
				// the file is reached.
				while ((line = sr.ReadLine()) != null) 
				{
					if (line.Length > MaxLineLength)
					{
						throw new InvalidOperationException(
							string.Format("File contains a line greater than {0} characters.",
							MaxLineLength.ToString()));
					}
					this._lines.Add(new TextLine(line));
				}
			}
		}
		#region IDiffList Members

		public int Count()
		{
			return this._lines.Count;
		}

		public IComparable GetByIndex(int index)
		{
			return (TextLine)this._lines[index];
		}

		#endregion
	
	}
}