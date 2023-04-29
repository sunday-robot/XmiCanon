using System.Collections.Immutable;

namespace XmiCanon2
{
    /// <summary>
    /// immutable<br/>
    /// XmiInnerNodeと、XmiLeafNodeの基底クラス。
    /// ノード名と属性の集合を持つ。
    /// </summary>
    public abstract class AbstractXmiNode
    {
        public readonly XmiName Name;
        public readonly ImmutableDictionary<XmiName, string> Attributes;
        public readonly IReadOnlyCollection<XmiAttribute> SortedAttributes;

        protected AbstractXmiNode(XmiName name, List<XmiAttribute> attributes)
        {
            Name = name;

            var dictionary = new Dictionary<XmiName, string>();
            foreach (var attribute in attributes)
            {
                dictionary[attribute.Name] = attribute.Value;
            }
            Attributes = dictionary.ToImmutableDictionary();

            var list = new List<XmiAttribute>(attributes);
            list.Sort((XmiAttribute a, XmiAttribute b) =>
            {
                var r = a.Name.Space.CompareTo(b.Name.Space);
                if (r != 0)
                    return r;
                return a.Name.Local.CompareTo(b.Name.Local);
            });
            SortedAttributes = list.ToImmutableArray();
        }
    }
}
