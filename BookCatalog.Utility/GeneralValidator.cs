using System.Text.RegularExpressions;
namespace BookCatalog.Utility
{
	public static class GeneralValidator
	{
		public static bool IsStringTooShort (string? str, uint minLength) 
		{
			return Regex.Replace(str ?? "", @"[\s]", "").Length < 3;
		}
	}
}
