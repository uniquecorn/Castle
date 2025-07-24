using Sirenix.OdinInspector;

namespace Castle.Core
{
    [System.Serializable]
    public abstract class CastleValueRange : CastleRange
    {
        public enum ConditionCheck
        {
            MoreOrEqual,
            Equal,
            Less
        }
        public enum ValueTypeCheck
        {
            None,
            Simple,
            SimpleValue,
            Value
        }
        [ShowIf("UseValue",ValueTypeCheck.Value)]
        public ConditionCheck checkType;
        public int value;
        protected virtual ValueTypeCheck UseValue => ValueTypeCheck.None;
    }
}