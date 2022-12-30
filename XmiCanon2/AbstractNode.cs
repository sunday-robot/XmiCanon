using System.Collections.Immutable;

namespace XmiCanon2
{
    public abstract class AbstractNode
    {
        public readonly string Name;
        public readonly IReadOnlyCollection<(string, string)> SortedAttributes;
        public string? GetAttribute(string name)
        {
            foreach (var attribute in SortedAttributes)
            {
                if (attribute.Item1.Equals(name))
                {
                    return attribute.Item2;
                }
            }
            return null;
        }

        protected AbstractNode(string name, List<(string, string)> attributes)
        {
            Name = name;
            var list = new List<(string, string)>(attributes);
            list.Sort(((string, string) a, (string, string) b) => a.Item1.CompareTo(b.Item1));
            SortedAttributes = list.ToImmutableArray();
        }
    }
}
