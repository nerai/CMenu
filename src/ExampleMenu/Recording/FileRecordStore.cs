using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ExampleMenu.Recording
{
	/// <summary>
	/// Stores records to files
	/// </summary>
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
			if (name == null) {
				throw new ArgumentNullException ("name");
			}
			if (lines == null) {
				throw new ArgumentNullException ("lines");
			}

			Directory.CreateDirectory (RecordDirectory);
			var path = RecordDirectory + name;
			File.WriteAllLines (path, lines);
		}

		/// <summary>
		/// Retrieves the specified record.
		///
		/// The record chosen is the first one to match, in order, any of the following:
		/// a) an exact match
		/// b) a record name starting with the specified name
		/// c) a record name containing the specified name
		/// </summary>
		public IEnumerable<string> GetRecord (string name)
		{
			if (name == null) {
				throw new ArgumentNullException ("name");
			}

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
