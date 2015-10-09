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
			var path = RecordDirectory + name;
			File.WriteAllLines (path, lines);
		}

		public IEnumerable<string> GetRecord (string name)
		{
			var path = RecordDirectory + name;
			if (!File.Exists (path)) {
				name = GetRecordNames ().FirstOrDefault (n => n.StartsWith (name, StringComparison.InvariantCultureIgnoreCase));
				name = name ?? GetRecordNames ().FirstOrDefault (n => n.Contains (name));
				if (name == null) {
					return null;
				}
				path = RecordDirectory + name;
			}
			var lines = File.ReadAllLines (path);
			return lines;
		}

		public IEnumerable<string> GetRecordNames ()
		{
			if (!Directory.Exists (RecordDirectory)) {
				return new string[0];
			}
			var files = Directory
				.EnumerateFiles (RecordDirectory)
				.Select (f => f.Substring (RecordDirectory.Length));
			return files;
		}
	}
}
