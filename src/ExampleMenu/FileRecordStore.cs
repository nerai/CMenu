using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ExampleMenu
{
	public class FileRecordStore : IRecordStore
	{
		private readonly string _Dir;

		public FileRecordStore (string dir = ".\\Records\\") // todo
		{
			_Dir = dir;
			Directory.CreateDirectory (_Dir);
		}

		public void AddRecord (string name, IEnumerable<string> lines)
		{
			File.WriteAllLines (_Dir + name + ".txt", lines);
		}

		public IEnumerable<string> GetRecord (string name)
		{
			var lines = File.ReadAllLines (".\\Records\\" + name + ".txt");
			return lines;
		}

		public IEnumerable<string> GetRecordNames ()
		{
			throw new NotImplementedException (); // todo
		}
	}
}
