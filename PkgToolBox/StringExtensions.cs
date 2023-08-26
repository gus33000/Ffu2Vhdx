using System;

namespace Microsoft.Composition.ToolBox
{
	public static class StringExtensions
	{
		public static bool EqualsIgnoreCase(this string A_0, string A_1)
		{
			return string.Equals(A_0, A_1, StringComparison.OrdinalIgnoreCase);
		}

		public static bool StartsWithIgnoreCase(this string A_0, string A_1)
		{
			return A_0.StartsWith(A_1, StringComparison.OrdinalIgnoreCase);
		}

		public static int IndexOfIgnoreCase(this string A_0, string A_1)
		{
			return A_0.IndexOf(A_1, StringComparison.OrdinalIgnoreCase);
		}

		public static int LastIndexOfIgnoreCase(this string A_0, string A_1)
		{
			return A_0.LastIndexOf(A_1, StringComparison.OrdinalIgnoreCase);
		}
	}
}
