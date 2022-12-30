namespace XmiCanon2
{
    public static class XmiPrinter
    {
        public static void Print(InnerNode root)
        {
            Console.WriteLine($"{root.Name}");
            PrintInnerNodeBody(root, "");
        }

        static void PrintInnerNode(InnerNode node, string indent, int index)
        {
            Console.WriteLine($"{indent}[{index}] {node.Name}");
            PrintInnerNodeBody(node, indent);
        }

        static void PrintInnerNodeBody(InnerNode node, string indent)
        {
            PrintAttributes(node, indent);
            var index = 0;
            foreach (var child in node.Children)
            {
                if (child is InnerNode innerNodeChild)
                {
                    PrintInnerNode(innerNodeChild, indent + "  ", index);
                }
                else
                {
                    PrintLeafNode((LeafNode)child, indent + "  ", index);
                }
                index++;
            }
        }

        static void PrintAttributes(AbstractNode node, string indent)
        {
            foreach (var attribute in node.SortedAttributes)
            {
                Console.WriteLine(indent + "    @" + attribute.Item1 + " = " + attribute.Item2);
            }
        }

        private static void PrintLeafNode(LeafNode node, string indent, int index)
        {
            if (node.SortedAttributes.Count > 0)
            {
                // 属性を持つ場合は、中間ノードと同様に名前と属性を出力する。
                Console.WriteLine($"{indent}[{index}]{node.Name}");

                // まずは中間の節と同じ形式で属性を出力する。
                PrintAttributes(node, indent);

                // 属性もテキストノードも持つ葉は、多分存在しないが、もしある場合は属性と同様に出力する。
                if (node.Text.Length > 0)
                {
                    Console.WriteLine($"    {indent}(text) = \"{node.Text}\"");
                }
            }
            else
            {
                // 属性を持たない場合は一つの行にノードの名前と値を出力する。
                Console.WriteLine($"{indent}[{index}] {node.Name} = \"{node.Text}\"");
            }
        }
    }
}
