using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace XmiCanon2
{
    public static class XmiPrinter2
    {
        static readonly Dictionary<string, string> NameSpaceAliaseDictionary = new() {
            {"http://www.eclipse.org/ui/2010/UIModel/application/ui/advanced", "advanced"},
            {"http://www.eclipse.org/ui/2010/UIModel/application", "application"},
            {"http://www.eclipse.org/ui/2010/UIModel/application/ui/basic", "basic"},
            {"http://www.eclipse.org/ui/2010/UIModel/application/ui/menu", "menu"},
            {"http://www.omg.org/XMI", "xmi"},
            {"http://www.w3.org/2001/XMLSchema-instance", "xsl"},
        };

        public static void Print(StreamWriter writer, AbstractXmiNode root, ImmutableDictionary<string, string> xmiIdToCanonId)
        {
            PrintNode(writer, root, "", "", xmiIdToCanonId);
        }

        static void PrintNode(StreamWriter writer, AbstractXmiNode node, string indent, string index, ImmutableDictionary<string, string> xmiIdToCanonId)
        {
            if (node is XmiInnerNode)
            {
                PrintInnerNode(writer, (XmiInnerNode)node, indent, index, xmiIdToCanonId);
            }
            else
            {
                PrintLeafNode(writer, (XmiLeafNode)node, indent, index, xmiIdToCanonId);
            }
        }

        static void PrintInnerNode(StreamWriter writer, XmiInnerNode node, string indent, string index, ImmutableDictionary<string, string> xmiIdToCanonId)
        {
            PrintName(writer, indent, index, node.Name);
            writer.WriteLine();
            PrintAttributes(writer, node, indent, xmiIdToCanonId);
            var childIndex = 0;
            foreach (var child in node.Children)
            {
                PrintNode(writer, child, indent + "  ", $"[{childIndex}] ", xmiIdToCanonId);
                childIndex++;
            }
        }

        private static void PrintLeafNode(StreamWriter writer, XmiLeafNode node, string indent, string index, ImmutableDictionary<string, string> xmiIdToCanonId)
        {
            PrintName(writer, indent, index, node.Name);
            if (node.SortedAttributes.Count > 0)
            {
                // 属性を持つ場合は、中間ノードと同様に名前と属性を出力する。
                writer.WriteLine();
                PrintAttributes(writer, node, indent, xmiIdToCanonId);

                // 属性もテキストノードも持つ葉は、多分存在しないが、もしある場合は属性と同様に出力する。
                if (node.Text.Length > 0)
                {
                    writer.WriteLine($"    {indent}(text) = \"{FormatText(node.Text)}\"");
                }
            }
            else
            {
                // 属性を持たない場合は一つの行にノードの名前と値を出力する。
                writer.WriteLine($" = \"{node.Text}\"");
            }
        }

        static void PrintName(StreamWriter writer, string indent, string index, XmiName name)
        {
            writer.Write($"{indent}{index}{FormatName(name)}");
        }

        static void PrintAttributes(StreamWriter writer, AbstractXmiNode node, string indent, ImmutableDictionary<string, string> xmiIdToCanonId)
        {
            foreach (var attribute in node.SortedAttributes)
            {
                var value = FormatText(attribute.Value);
                if (xmiIdToCanonId.ContainsKey(value))
                {
                    value = xmiIdToCanonId[value];
                }
                writer.WriteLine(indent + "    @" + FormatName(attribute.Name) + " = " + value);
            }
        }

        static string FormatName(XmiName name)
        {
            if (name.Space.Length > 0)
            {
                var nameSpace = ReplaceWithAlias(name.Space);
                return nameSpace + ":" + name.Local;
            }
            else
                return name.Local;
        }

        static string ReplaceWithAlias(string nameSpace)
        {
            if (NameSpaceAliaseDictionary.ContainsKey(nameSpace))
                return NameSpaceAliaseDictionary[nameSpace];
            else
                return nameSpace;
        }

        /// <summary>
        /// 属性値、テキストノードの整形(行頭、行末の空白削除と改行文字の削除)を行う。
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        static string FormatText(string text)
        {
            var r = "";
            var reader = new StringReader(text);
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (r.Length > 0)
                    r += " " + line.Trim();
                else
                    r = line.Trim();
            }
            return r;
        }
    }
}
