using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ExampleMenu
{
	public class FileRecordStore : IRecordStore
	{
		public FileRecordStore ()
		{
			RecordDirectory = ".\\Records\\";
		}

		public string RecordDirectory
		{
			get;
			set;
		}

		public void AddRecord (string name, IEnumerable<string> lines)
		{
			Directory.CreateDirectory (RecordDirectory);
			var path = RecordDirectory + name + ".txt";
			File.WriteAllLines (path, lines);
		}

		public IEnumerable<string> GetRecord (string name)
		{
			var path = RecordDirectory + name + ".txt";
			if (!File.Exists (path)) {
				return null;
			}
			var lines = File.ReadAllLines (path);
			return lines;
		}

		public IEnumerable<string> GetRecordNames ()
		{
			if (!Directory.Exists (RecordDirectory)) {
				return new string[0];
			}
			var files = Directory.EnumerateFiles (RecordDirectory);
			return files;
		}
	}
}
