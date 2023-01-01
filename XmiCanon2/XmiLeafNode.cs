namespace XmiCanon2
{
    /// <summary>
    /// 葉ノード<br/>
    /// 根、中間ノードとの差異は以下の2点。<br/>
    /// ・子ノードを持たない<br/>
    /// ・Textデータを持つ<br/>
    /// </summary>
    public sealed class XmiLeafNode : AbstractXmiNode
    {
        public readonly string Text;

        public XmiLeafNode(XmiName name, List<XmiAttribute> attributes, string text) : base(name, attributes)
        {
            Text = text;
        }
    }
}
