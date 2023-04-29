namespace XmiCanon2
{
    /// <summary>
    /// immutable<br/>
    /// 要素および属性の名前のためのもので、名前空間とローカルな名前の二つのプロパティを持つ。
    /// </summary>
    public sealed class XmiName
    {
        public readonly string Space;
        public readonly string Local;

        public XmiName(string space, string local)
        {
            Space = space;
            Local = local;
        }

        public override bool Equals(object? obj)
        {
            if (obj is XmiName xmiName)
            {
                return Space.Equals(xmiName.Space) && Local.Equals(xmiName.Local);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}
