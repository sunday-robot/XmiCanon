using System.Xml.Linq;

namespace XmiCanon2
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            Sample();
        }

        static void ParseWorkbenchXmi()
        {
            var xml = XElement.Load("workbench.xmi");
            foreach (var element in xml.Elements())
            {
                Console.Write(element);
            }
        }

        static void Sample()
        {
            //xmlファイルを指定する
            XDocument xml = XDocument.Load("sample.xml");

            PrintXElement(xml.Root, string.Empty);

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

        // XMLドキュメントの本来の姿ではないが、現実に使われている多くのXMLは以下のような構造になっている。
        // 根、root ............ XMLドキュメントに一つだけ存在する。それ以外は中間の節と同じ。
        // -- 中間の節、node ... XElement型。子供にXElementは持つが、テキストノードは持たない。
        // ----- 葉、leaf ...... XElement型。子供にXElementは持たず、テキストノードを一つだけ持つ。 

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
    }
}