namespace XmiCanon2
{
    public sealed class XmiName
    {
        public readonly string Space;
        public readonly string Local;

        public XmiName(string space, string local)
        {
            Space = space;
            Local = local;
        }
    }
}
