using System;
using System.Xml.Linq;

namespace XmiCanon2
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            //Sample();
            //ParseWorkbenchXmi();
            if (true)
            {
                var inputXmiFilePath = args[0];
                var outputXmiFilePath = inputXmiFilePath + ".txt";
                var root = XmiLoader.Load(inputXmiFilePath);
                var writer = new StreamWriter(outputXmiFilePath);
                var xmiIdToCanonId = CanonXmiIdCreater.Create(root);
                XmiPrinter2.Print(writer, root, xmiIdToCanonId);
                writer.Close(); // なぜかこれを行わないと書き込みバッファのフラッシュが行われず、ファイルが尻切れになってしまう。
            }
            if (false)
            {
                var root = XmiLoader.Load("nobody.xml");
                Console.WriteLine(root);
            }
        }

        static void ParseWorkbenchXmi()
        {
            var xml = XDocument.Load("workbench.xmi");
            PrintXElement2(xml.Root!, string.Empty, 0);
        }

        static void Sample()
        {
            //xmlファイルを指定する
            XDocument xml = XDocument.Load("sample.xml");

            PrintXElement2(xml.Root!, string.Empty, 0);

            ////メンバー情報のタグ内の情報を取得する
            //IEnumerable<XElement> infos = from item in xml.Elements("メンバー情報")
            //                              select item;

            ////メンバー情報分ループして、コンソールに表示
            //foreach (XElement info in infos)
            //{
            //    Console.Write(info.Element("名前").Value + @",");
            //    Console.Write(info.Element("住所").Value + @",");
            //    Console.WriteLine(info.Element("年齢").Value);
            //}
        }


        static void PrintXElement(XElement element, string indent)
        {
            Console.Write(indent + element.Name + "{");
            foreach (XAttribute attribute in element.Attributes())
            {
                Console.Write($"{attribute.Name}=\"{attribute.Value}\", ");
            }
            Console.WriteLine("}");
            foreach (XNode chidNode in element.Nodes())
            {
                if (chidNode is XElement childElement)
                {
                    PrintXElement(childElement, indent + "  ");
                }
                else if (chidNode is XText childText)
                {
                    Console.WriteLine(indent + $"  text=\"{childText.Value.Trim()}\"");
                }
                else
                {
                    Console.WriteLine(chidNode);
                }
            }
        }

        // XMLドキュメントの本来の姿ではないが、現実に使われている多くのXMLは以下のような構造になっている。
        // 根、root ............ XMLドキュメントに一つだけ存在する。それ以外は中間の節と同じ。
        // -- 中間の節、node ... XElement型。子供にXElementは持つが、テキストノードは持たない。
        // ----- 葉、leaf ...... XElement型。子供にXElementは持たず、テキストノードを一つだけ持つ。 
        static void PrintXElement2(XElement element, string indent, int index)
        {
            Console.Write($"{indent}[{index}]{element.Name.LocalName}");   // フルネームでは長すぎるので名前空間のない名前を出力する
            if (element.HasElements)
            {
                // 中間の節の場合：
                // 中間の節の場合は、子要素は存在するが、テキストノードは存在しないと決め打ちしている。

                Console.WriteLine();
                if (element.HasAttributes)
                {
                    PrintAttributes(element, indent);
                }

                // 中間の節の場合、テキストノードは存在しないと決め打ちし、各子エレメントに対して再起呼び出しする。
                var i = 0;
                foreach (XElement childElement in element.Elements())
                {
                    PrintXElement2(childElement, indent + "  ", i++);
                }
            }
            else
            {
                // 葉の場合：
                // 葉の場合は、テキストノードはないか、一つだけ存在すると決め打ちしている。

                if (element.HasAttributes)
                {
                    // 属性を持つ場合は、テキストノードの値を属性の一つであるかのようにして出力する。

                    // まずは中間の節と同じ形式で属性を出力する。
                    Console.WriteLine();
                    PrintAttributes(element, indent);

                    // 属性もテキストノードも持つ葉は、多分存在しない。
                    if (element.FirstNode != null)
                    {
                        string value = ((XText)element.FirstNode).Value;
                        Console.WriteLine($"    {indent}(text) = \"{FormatText(value)}\"");
                    }
                }
                else
                {
                    // 属性を持たない場合は一つの行にノードの名前と値を出力する。
                    if (element.FirstNode != null)
                    {
                        string value = ((XText)element.FirstNode).Value;
                        Console.Write($" = \"{FormatText(value)}\"");
                    }
                    Console.WriteLine();
                }
            }
        }

        static void PrintAttributes(XElement element, string indent)
        {
            foreach (XAttribute attribute in element.Attributes())
            {
                Console.WriteLine($"    {indent}@{attribute.Name.LocalName} = \"{FormatText(attribute.Value)}\", ");
            }
        }

        static string FormatText(string text)
        {
            return ConvertToSingleLineText(text.Trim());
        }

        static string ConvertToSingleLineText(string multilineText)
        {
            var singlelineText = "";
            var sr = new StringReader(multilineText);
            string? l;
            while ((l = sr.ReadLine()) != null)
            {
                singlelineText += l;
            }
            return singlelineText;
        }
    }
}