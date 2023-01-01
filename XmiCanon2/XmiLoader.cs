using System.Xml.Linq;

namespace XmiCanon2
{
    public static class XmiLoader
    {
        public static XmiInnerNode Load(string xmlFilePath)
        {
            var xml = XDocument.Load(xmlFilePath);
            return CreateInnerNode(xml.Root);
        }

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
            foreach (XElement childElement in element.Elements())
            {
                AbstractXmiNode child;
                if (childElement.HasElements)
                {
                    child = CreateInnerNode(childElement);
                }
                else
                {
                    child = CreateLeafNode(childElement);
                }
                children.Add(child);
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
                string value = ((XText)firstNode).Value;
                text = FormatText(value);
            }
            return new XmiLeafNode(name, attributes, text);
        }

        static XmiName CreateName(XName xName) => new(xName.NamespaceName, xName.LocalName);

        static (XmiName, List<XmiAttribute>) GetNameAndAttributes(XElement element)
        {
            var name = CreateName(element.Name);
            var attributes = new List<XmiAttribute>();
            foreach (XAttribute attribute in element.Attributes())
            {
                attributes.Add(new XmiAttribute(CreateName(attribute.Name), FormatText(attribute.Value)));
            };
            return (name, attributes);
        }

        static string FormatText(string text)
        {
            var r = "";
            var reader = new StringReader(text);
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                r += line.Trim();
            }
            return r;
        }
    }
}
