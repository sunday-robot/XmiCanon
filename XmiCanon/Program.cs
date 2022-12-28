using System.Text.RegularExpressions;

namespace XmiCanon2
{
    public sealed class Program
    {
        static readonly Regex XmiIdKeyValueRegex = new("xmi:id=\"([^\"]+)\"");
        static readonly Regex KeyValueRegex = new("([\\w:]+)=\"([^\"]+)\"");

        public static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            if (true)
            {
                foreach (string inputXmiFilePath in args)
                {
                    Console.WriteLine(inputXmiFilePath);
                    var reader = new StreamReader(inputXmiFilePath);
                    var writer = new StreamWriter(inputXmiFilePath + ".out.xml");

                    CanonicalizeXmiFile(reader, writer);
                }
            }
            else
            {
                var s = "xxx xmi:id=\"_abc_\" yyy ABC=\"_abc_\" DEF=\"_def_\"";
                Dictionary<string, string> xmiIdToCanonId = new();
                AddCanonId(xmiIdToCanonId, s);
                var canonS = ReplaceWithCanonId(s, xmiIdToCanonId);
                Console.WriteLine(s);
                Console.WriteLine("->" + canonS);
            }
        }

        static void CanonicalizeXmiFile(StreamReader reader, TextWriter writer)
        {
            var xmiIdToCanonId = CreateXmiIdToCanonId(reader);
            reader.BaseStream.Position = 0;
            WriteCanonIdXmiFile(reader, xmiIdToCanonId, writer);
        }

        static Dictionary<string, string> CreateXmiIdToCanonId(TextReader reader)
        {
            Dictionary<string, string> xmiIdToCanonId = new();
            string? s;
            while ((s = reader.ReadLine()) != null)
            {
                AddCanonId(xmiIdToCanonId, s);
            }

            return xmiIdToCanonId;
        }

        private static void AddCanonId(Dictionary<string, string> xmiIdToCanonId, string s)
        {
            var matches = XmiIdKeyValueRegex.Matches(s);
            foreach (Match match in matches)
            {
                var value = match.Groups[1].Value;
                if (xmiIdToCanonId.ContainsKey(value))
                {
                    continue;
                }

                xmiIdToCanonId[value] = string.Format($"_{xmiIdToCanonId.Count + 1}_");
            }
        }
        private static void WriteCanonIdXmiFile(StreamReader reader, Dictionary<string, string> xmiIdToCanonId, TextWriter writer)
        {
            string? s;
            while ((s = reader.ReadLine()) != null)
            {
                var canonS = ReplaceWithCanonId(s, xmiIdToCanonId);
                writer.WriteLine(canonS);
            }
        }

        private static object ReplaceWithCanonId(string s, Dictionary<string, string> xmiIdToCanonId)
        {
            string canonS = "";
            var sIndex = 0;
            var matches = KeyValueRegex.Matches(s);
            foreach (Match match in matches)
            {
                var keyGroup = match.Groups[1];
                var valueGroup = match.Groups[2];
                var value = valueGroup.Value;

                // 直前のマッチ箇所の次から、今回のマッチ個所の値の手前の二重引用符までを追加する。
                canonS += s[sIndex..valueGroup.Index];
                sIndex = match.Index + match.Length;

                string canonId;
                if (xmiIdToCanonId.ContainsKey(value))
                {
                    canonId = xmiIdToCanonId[value];
                }
                else
                {
                    canonId = value;
                }

                // 値と二重引用符を追加する。
                canonS += canonId + "\"";
            }
            return string.Concat(canonS, s.AsSpan(sIndex));
        }

    }
}