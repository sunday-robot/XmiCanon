using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmiCanon2
{
    internal class CanonXmiIdCreater
    {
        static readonly XmiName XmiIdAttributeName = new("http://www.omg.org/XMI", "id");
        static readonly XmiName ElementIdAttributeName = new("", "elementId");

        public static ImmutableDictionary<string, string> Create(AbstractXmiNode root)
        {
            var xmiIdToCanonId = new Dictionary<string, string>();
            int sequenceNo = 0;
            Create(root, xmiIdToCanonId, ref sequenceNo);
            return xmiIdToCanonId.ToImmutableDictionary();
        }

        static void Create(AbstractXmiNode node, Dictionary<string, string> xmiIdToCanonId, ref int sequenceNo)
        {
            var xmiId = GetXmiId(node);
            if (xmiId != null)
            {
                var elementId = GetElementId(node);
                if (elementId != null)
                {
                    if (xmiIdToCanonId.ContainsKey(xmiId))
                    {
                        Console.Error.WriteLine($"xmi:id が重複しています。xmi:id={xmiId}, 登録済みelementId={xmiIdToCanonId[xmiId]}, 重複elementId={elementId}");
                    }
                    else
                    {
                        xmiIdToCanonId.Add(xmiId, elementId);
                    }
                }
                else
                {
                    xmiIdToCanonId.Add(xmiId, string.Format($"_{sequenceNo}_"));
                    sequenceNo++;
                }
            }
            if (node is XmiInnerNode innerNode)
            {
                foreach (var child in innerNode.Children)
                {
                    Create(child, xmiIdToCanonId, ref sequenceNo);
                }
            }
        }

        static string? GetXmiId(AbstractXmiNode node) => Get(node, XmiIdAttributeName);

        static string? GetElementId(AbstractXmiNode node) => Get(node, ElementIdAttributeName);

        static string? Get(AbstractXmiNode node, XmiName name)
        {
            foreach (var attribute in node.SortedAttributes)
            {
                if (attribute.Name.Equals(name))
                    return attribute.Value;
            }
            return null;
        }
    }
}
