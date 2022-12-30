using System.Collections.Immutable;

namespace XmiCanon2
{
    /// <summary>
    /// 根、節ノード<br/>
    /// 葉ノードとの差異は以下の2点。<br/>
    /// ・子ノードを持つ<br/>
    /// ・Textデータを持たない<br/>
    /// </summary>
    public sealed class InnerNode : AbstractNode
    {
        public IReadOnlyCollection<AbstractNode> Children;

        public InnerNode(string name, List<(string, string)> sortedAttributes, List<AbstractNode> children) : base(name, sortedAttributes)
        {
            Children = children.ToImmutableArray();
        }
    }
}
