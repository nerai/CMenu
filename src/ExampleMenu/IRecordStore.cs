using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExampleMenu
{
	/// <summary>
	/// A record store
	/// </summary>
	public interface IRecordStore
	{
		void AddRecord (string name, IEnumerable<string> lines);

		IEnumerable<string> GetRecord (string name);

		IEnumerable<string> GetRecordNames ();
	}
}
