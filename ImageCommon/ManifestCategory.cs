using System.Collections;
using System.Globalization;

namespace Microsoft.WindowsPhone.Imaging
{
    public class ManifestCategory
	{
		public string this[string A_1]
		{
			get
			{
				return (string)_keyValues[A_1];
			}
			set
			{
				if (_keyValues.ContainsKey(A_1))
				{
					_keyValues[A_1] = value;
					return;
				}
				if (A_1.Length > _maxKeySize)
				{
					_maxKeySize = A_1.Length;
				}
				_keyValues.Add(A_1, value);
			}
		}

		public string Category
		{
            get; set;
        }

		public string Name { get; private set; }

		public ManifestCategory(string A_1)
		{
			Name = A_1;
		}

		public ManifestCategory(string A_1, string A_2) : this(A_1)
		{
			Category = A_2;
		}

		internal bool GetBool(string A_1)
		{
			return GetBool(A_1, false);
		}

		internal bool GetBool(string A_1, bool A_2)
		{
			if (_keyValues.ContainsKey(A_1))
			{
				return bool.Parse(this[A_1]);
			}
			return A_2;
		}

		internal uint GetUInt32(string A_1)
		{
			return GetUInt32(A_1, 0U);
		}

		internal uint GetUInt32(string A_1, uint A_2)
		{
			if (_keyValues.ContainsKey(A_1))
			{
				return uint.Parse(this[A_1], CultureInfo.InvariantCulture);
			}
			return A_2;
		}

		internal ulong GetUInt64(string A_1)
		{
			return GetUInt64(A_1, 0UL);
		}

		internal ulong GetUInt64(string A_1, ulong A_2)
		{
			if (_keyValues.ContainsKey(A_1))
			{
				return ulong.Parse(this[A_1], CultureInfo.InvariantCulture);
			}
			return A_2;
		}

		internal string GetString(string A_1)
		{
			return GetString(A_1, null);
		}

		internal string GetString(string A_1, string A_2)
		{
			if (_keyValues.ContainsKey(A_1))
			{
				return this[A_1];
			}
			return A_2;
		}

		private int _maxKeySize;

		private Hashtable _keyValues = new Hashtable();
	}
}
