using System.Xml.Linq;

namespace XmiCanon2
{
    public static class XmiLoader
    {
        public static AbstractXmiNode Load(string xmlFilePath)
        {
            var xml = XDocument.Load(xmlFilePath);
            return CreateNode(xml.Root!);
        }

        static AbstractXmiNode CreateNode(XElement element) => element.HasElements ?  CreateInnerNode(element) :  CreateLeafNode(element);

        // XMLドキュメントの本来の姿ではないが、現実に使われている多くのXMLは以下のような構造になっている。
        // 根、root ............ XMLドキュメントに一つだけ存在する。それ以外は中間の節と同じ。
        // -- 中間の節、node ... XElement型。子供にXElementは持つが、テキストノードは持たない。
        // ----- 葉、leaf ...... XElement型。子供にXElementは持たず、テキストノードは持たないか、一つだけ持つ。 
        static XmiInnerNode CreateInnerNode(XElement element)
        {
            // 中間の節の場合：
            // 中間の節の場合は、子要素は存在するが、テキストノードは存在しないと決め打ちしている。
            var (name, attributes) = GetNameAndAttributes(element);
            var children = new List<AbstractXmiNode>();
            foreach (XNode childNode in element.Nodes())
            {
                if (childNode is XElement childElement)
                {
                    children.Add(CreateNode(childElement));
                }
                else
                {
                    Console.Error.WriteLine("Warning: 子にテキストノードと要素のどちらも存在しています。(XMLとしては有効だが、あまり一般的ではない。)");
                }
            }
            return new XmiInnerNode(name, attributes, children);
        }

        static XmiLeafNode CreateLeafNode(XElement element)
        {
            // 葉の場合：
            // 葉の場合は、テキストノードはないか、一つだけ存在すると決め打ちしている。
            var (name, attributes) = GetNameAndAttributes(element);
            string text;
            var firstNode = element.FirstNode;
            if (firstNode == null)
            {
                text = string.Empty;
            }
            else
            {
                text = ((XText)firstNode).Value;
            }
            return new XmiLeafNode(name, attributes, text);
        }

        static (XmiName, List<XmiAttribute>) GetNameAndAttributes(XElement element)
        {
            var name = CreateName(element.Name);
            var attributes = new List<XmiAttribute>();
            foreach (XAttribute attribute in element.Attributes())
            {
                attributes.Add(new XmiAttribute(CreateName(attribute.Name), attribute.Value));
            };
            return (name, attributes);
        }

        static XmiName CreateName(XName xName) => new(xName.NamespaceName, xName.LocalName);
    }
}
