using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.WindowsPhone.Imaging
{
	public class FullFlashUpdateManifest
	{
		internal FullFlashUpdateManifest(FullFlashUpdateImage A_1)
		{
			_ffuImage = A_1;
		}

		internal FullFlashUpdateManifest(FullFlashUpdateImage A_1, byte[] A_2) : this(A_1)
		{
			using (MemoryStream memoryStream = new MemoryStream(A_2))
			{
				using (StreamReader streamReader = new StreamReader(memoryStream, Encoding.ASCII))
				{
					Regex regex = new Regex("^\\s*\\[(?<category>[^\\]]+)\\]\\s*$");
					Regex regex2 = new Regex("^\\s*(?<key>[^=\\s]+)\\s*=\\s*(?<value>.*)(\\s*$)");
					Match match = null;
					ManifestCategory manifestCategory = null;
					while (!streamReader.EndOfStream)
					{
						string text = streamReader.ReadLine();
						if (regex.IsMatch(text))
						{
							match = null;
							string value = regex.Match(text).Groups["category"].Value;
							ProcessCategory(manifestCategory);
							if (string.Compare(value, "StoragePool", StringComparison.Ordinal) == 0)
							{
								manifestCategory = new ManifestCategory("StoragePool", "StoragePool");
							}
							else if (string.Compare(value, "StoragePoolStore", StringComparison.Ordinal) == 0)
							{
								manifestCategory = new ManifestCategory("StoragePoolStore", "StoragePoolStore");
							}
							else if (string.Compare(value, "StoragePoolPartition", StringComparison.Ordinal) == 0)
							{
								manifestCategory = new ManifestCategory("StoragePoolPartition", "StoragePoolPartition");
							}
							else if (string.Compare(value, "Store", StringComparison.Ordinal) == 0)
							{
								manifestCategory = new ManifestCategory("Store", "Store");
							}
							else if (string.Compare(value, "Partition", StringComparison.Ordinal) == 0)
							{
								manifestCategory = new ManifestCategory("Partition", "Partition");
							}
							else
							{
								manifestCategory = AddCategory(value, value);
							}
						}
						else if (manifestCategory != null && regex2.IsMatch(text))
						{
							match = null;
							Match match2 = regex2.Match(text);
							manifestCategory[match2.Groups["key"].Value] = match2.Groups["value"].Value;
							if (match2.Groups["key"].ToString() == "Description")
							{
								match = match2;
							}
						}
						else if (match != null)
						{
							ManifestCategory manifestCategory2 = manifestCategory;
							string value2 = match.Groups["key"].Value;
							manifestCategory2[value2] = manifestCategory2[value2] + "\r\n" + text;
						}
					}
					ProcessCategory(manifestCategory);
				}
			}
		}

		internal ManifestCategory AddCategory(string A_1, string A_2)
		{
			if (this[A_1] != null)
			{
				throw new ImageCommonException(CodeSite.StorageService, ErrorCategory.IncorrectUserInput, "Cannot add duplicate categories to a manifest.");
			}
			ManifestCategory manifestCategory = new ManifestCategory(A_1, A_2);
			_categories.Add(manifestCategory);
			return manifestCategory;
		}

		internal ManifestCategory this[string A_1]
		{
			get
			{
				foreach (object obj in _categories)
				{
					ManifestCategory manifestCategory = (ManifestCategory)obj;
					if (string.Compare(manifestCategory.Name, A_1, StringComparison.Ordinal) == 0)
					{
						return manifestCategory;
					}
				}
				return null;
			}
		}

		internal void Validate(string A_1)
		{
			if (this["FullFlash"] == null)
			{
				throw new ImageCommonException(CodeSite.StorageService, ErrorCategory.BadContent, "Missing 'FullFlash' or 'Image' category in the manifest");
			}
			string text = this["FullFlash"]["Version"];
			if (text == null)
			{
				throw new ImageCommonException(CodeSite.StorageService, ErrorCategory.BadContent, "Missing 'Version' name/value pair in the 'FullFlash' category.");
			}
			if (!text.Equals(A_1, StringComparison.OrdinalIgnoreCase))
			{
				throw new ImageCommonException(CodeSite.StorageService, ErrorCategory.BadContent, string.Concat(new string[]
				{
					"'Version' value (",
					text,
					") does not match current version of ",
					A_1,
					"."
				}));
			}
		}

		private void ProcessCategory(ManifestCategory A_1)
		{
			if (A_1 != null)
			{
				if (string.CompareOrdinal(A_1.Name, "StoragePool") == 0)
				{
					_ffuImage.AddStoragePool(A_1);
					A_1 = null;
					return;
				}
				if (string.CompareOrdinal(A_1.Name, "StoragePoolStore") == 0)
				{
					_ffuImage.StoragePools.Last().AddStore(A_1);
					A_1 = null;
					return;
				}
				if (string.CompareOrdinal(A_1.Name, "StoragePoolPartition") == 0)
				{
					_ffuImage.StoragePools.Last().Stores.Last().AddPartition(A_1);
					A_1 = null;
					return;
				}
				if (string.CompareOrdinal(A_1.Name, "Store") == 0)
				{
					_ffuImage.AddStore(A_1);
					A_1 = null;
					return;
				}
				if (string.CompareOrdinal(A_1.Name, "Partition") == 0)
				{
					_ffuImage.Stores.Last().AddPartition(A_1);
					A_1 = null;
				}
			}
		}

		private ArrayList _categories = new ArrayList(20);

		private FullFlashUpdateImage _ffuImage;
	}
}
