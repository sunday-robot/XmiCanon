using System.Collections.Immutable;

namespace XmiCanon2
{
    public abstract class AbstractXmiNode
    {
        public readonly XmiName Name;
        public readonly IReadOnlyCollection<XmiAttribute> SortedAttributes;
        public string? GetAttribute(XmiName name)
        {
            foreach (var attribute in SortedAttributes)
            {
                if (attribute.Name.Equals(name))
                {
                    return attribute.Value;
                }
            }
            return null;
        }

        protected AbstractXmiNode(XmiName name, List<XmiAttribute> attributes)
        {
            Name = name;
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
