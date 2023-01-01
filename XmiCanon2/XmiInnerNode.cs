using System.Collections.Immutable;

namespace XmiCanon2
{
    /// <summary>
    /// 根、節ノード<br/>
    /// 葉ノードとの差異は以下の2点。<br/>
    /// ・子ノードを持つ<br/>
    /// ・Textデータを持たない<br/>
    /// </summary>
    public sealed class XmiInnerNode : AbstractXmiNode
    {
        public IReadOnlyCollection<AbstractXmiNode> Children;

        public XmiInnerNode(XmiName name, List<XmiAttribute> sortedAttributes, List<AbstractXmiNode> children) : base(name, sortedAttributes)
        {
            Children = children.ToImmutableArray();
        }
    }
}
