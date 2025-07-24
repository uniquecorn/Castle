using System.IO;

namespace Castle
{
    public static class StringExtensions
    {
        public static string Shorten(this string value, int delete) => value[..^delete];

        public static string NameNoExtension(this FileInfo file) => file.Name.Shorten(file.Extension.Length);
        public static string Slugged(this string s) => Tools.Slug(s);
    }
}