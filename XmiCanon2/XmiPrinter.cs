using System.Collections.ObjectModel;

namespace XmiCanon2
{
    public static class XmiPrinter
    {
        static readonly Dictionary<string, string> NameSpaceAliaseDictionary = new() {
            {"http://www.eclipse.org/ui/2010/UIModel/application/ui/advanced", "advanced"},
            {"http://www.eclipse.org/ui/2010/UIModel/application", "application"},
            {"http://www.eclipse.org/ui/2010/UIModel/application/ui/basic", "basic"},
            {"http://www.eclipse.org/ui/2010/UIModel/application/ui/menu", "menu"},
            {"http://www.omg.org/XMI", "xmi"},
            {"http://www.w3.org/2001/XMLSchema-instance", "xsl"},
        };

        public static void Print(StreamWriter writer, XmiInnerNode root)
        {
            writer.WriteLine($"{FormatName(root.Name)}");
            PrintInnerNodeBody(writer, root, "");
        }

        static void PrintInnerNode(StreamWriter writer, XmiInnerNode node, string indent, int index)
        {
            writer.WriteLine($"{indent}[{index}] {FormatName(node.Name)}");
            PrintInnerNodeBody(writer, node, indent);
        }

        static void PrintInnerNodeBody(StreamWriter writer, XmiInnerNode node, string indent)
        {
            PrintAttributes(writer, node, indent);
            var index = 0;
            foreach (var child in node.Children)
            {
                if (child is XmiInnerNode innerNodeChild)
                {
                    PrintInnerNode(writer, innerNodeChild, indent + "  ", index);
                }
                else
                {
                    PrintLeafNode(writer, (XmiLeafNode)child, indent + "  ", index);
                }
                index++;
            }
        }

        static void PrintAttributes(StreamWriter writer, AbstractXmiNode node, string indent)
        {
            foreach (var attribute in node.SortedAttributes)
            {
                writer.WriteLine(indent + "    @" + FormatName(attribute.Name) + " = " + attribute.Value);
            }
        }

        private static void PrintLeafNode(StreamWriter writer, XmiLeafNode node, string indent, int index)
        {
            if (node.SortedAttributes.Count > 0)
            {
                // 属性を持つ場合は、中間ノードと同様に名前と属性を出力する。
                writer.WriteLine($"{indent}[{index}] {FormatName(node.Name)}");

                // まずは中間の節と同じ形式で属性を出力する。
                PrintAttributes(writer, node, indent);

                // 属性もテキストノードも持つ葉は、多分存在しないが、もしある場合は属性と同様に出力する。
                if (node.Text.Length > 0)
                {
                    writer.WriteLine($"    {indent}(text) = \"{node.Text}\"");
                }
            }
            else
            {
                // 属性を持たない場合は一つの行にノードの名前と値を出力する。
                writer.WriteLine($"{indent}[{index}] {FormatName(node.Name)} = \"{node.Text}\"");
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
    }
}
