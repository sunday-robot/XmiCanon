using System.Collections.Immutable;
using System.Xml.Linq;

namespace XmiCanon2
{
    public static class XmiLoader
    {
        public static InnerNode Load(string xmlFilePath)
        {
            var xml = XDocument.Load(xmlFilePath);
            return CreateInnerNode(xml.Root);
        }

        // XMLドキュメントの本来の姿ではないが、現実に使われている多くのXMLは以下のような構造になっている。
        // 根、root ............ XMLドキュメントに一つだけ存在する。それ以外は中間の節と同じ。
        // -- 中間の節、node ... XElement型。子供にXElementは持つが、テキストノードは持たない。
        // ----- 葉、leaf ...... XElement型。子供にXElementは持たず、テキストノードを一つだけ持つ。 
        static InnerNode CreateInnerNode(XElement element)
        {
            var children = new List<AbstractNode>();
            foreach (XElement childElement in element.Elements())
            {
                children.Add(CreateNode(childElement));
            }
            var (name, attributes) = GetNameAndAttributes(element);
            return new InnerNode(name, attributes, children);
        }

        static (string, List<(string, string)>) GetNameAndAttributes(XElement element)
        {
            var name = element.Name.LocalName;
            var attributes = new List<(string, string)>();
            foreach (XAttribute attribute in element.Attributes())
            {
                attributes.Add((attribute.Name.LocalName, FormatText(attribute.Value)));
            };
            return (name, attributes);
        }

        static AbstractNode CreateNode(XElement element)
        {
            if (element.HasElements)
            {
                // 中間の節の場合：
                // 中間の節の場合は、子要素は存在するが、テキストノードは存在しないと決め打ちしている。
                return CreateInnerNode(element);
            }
            else
            {
                // 葉の場合：
                // 葉の場合は、テキストノードはないか、一つだけ存在すると決め打ちしている。
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
                var (name, attributes) = GetNameAndAttributes(element);
                return new LeafNode(name, attributes, text);
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
            string l;
            while ((l = sr.ReadLine()) != null)
            {
                singlelineText += l;
            }
            return singlelineText;
        }
    }
}
