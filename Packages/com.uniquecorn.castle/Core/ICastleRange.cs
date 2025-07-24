namespace Castle.Core
{
    public interface ICastleRange
    {
        public enum ConditionOp
        {
            And,
            Or
        }
        string Label { get; }
        ConditionOp ArrayCheck => ConditionOp.And;
    }
}