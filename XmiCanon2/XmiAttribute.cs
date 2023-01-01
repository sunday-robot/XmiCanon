namespace XmiCanon2
{
    public sealed class XmiAttribute
    {
        public readonly XmiName Name;
        public readonly string Value;

        public XmiAttribute(XmiName name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
