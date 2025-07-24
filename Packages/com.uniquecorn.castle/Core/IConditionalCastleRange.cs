namespace Castle.Core
{
    public interface IConditionalCastleRange<in T> : ICastleRange
    {
        bool Check(T variable);
    }
}