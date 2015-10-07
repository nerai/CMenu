using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleMenu
{
	public class DefaultDictionary<TKey, TValue> where TValue : class
	{
		private Dictionary<TKey, TValue> _D;

		public TValue Default = null;

		public TValue this[TKey key]
		{
			get
			{
				TValue v;
				if (key != null) {
					if (_D.TryGetValue (key, out v)) {
						return v;
					}
				}
				return Default;
			}
			set
			{
				if (key == null) {
					Default = value;
				}
				else {
					_D[key] = value;
				}
			}
		}
	}
}
