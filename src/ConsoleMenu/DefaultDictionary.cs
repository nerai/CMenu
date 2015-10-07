using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleMenu
{
	// todo codedoc
	public class DefaultDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
		where TValue : class
	{
		private Dictionary<TKey, TValue> _D = new Dictionary<TKey, TValue> ();

		public TValue Default = null;

		public DefaultDictionary ()
		{
		}

		public void SetComparer (IEqualityComparer<TKey> comparer)
		{
			_D = new Dictionary<TKey, TValue> (_D, comparer);
		}

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

		public bool TryGetValue (TKey key, out TValue value)
		{
			return _D.TryGetValue (key, out value);
		}

		public TValue TryGetValue (TKey key)
		{
			TValue v;
			_D.TryGetValue (key, out v);
			return v;
		}

		public void Add (TKey key, TValue value)
		{
			if (key == null) {
				Default = value;
			}
			else {
				_D.Add (key, value);
			}
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator ()
		{
			return _D.GetEnumerator ();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}

		public Dictionary<TKey, TValue>.ValueCollection Values
		{
			get
			{
				return _D.Values;
			}
		}
	}
}
