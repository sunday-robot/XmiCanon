namespace XmiCanon2
{
    /// <summary>
    /// 葉ノード<br/>
    /// 根、中間ノードとの差異は以下の2点。<br/>
    /// ・子ノードを持たない<br/>
    /// ・Textデータを持つ<br/>
    /// </summary>
    public sealed class LeafNode : AbstractNode
    {
        public readonly string Text;

        public LeafNode(string name, List<(string, string)> attributes, string text) : base(name, attributes)
        {
            Text = text;
        }
    }
}
