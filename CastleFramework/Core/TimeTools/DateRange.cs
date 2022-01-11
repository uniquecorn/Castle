using Castle.Core.Range;
using Sirenix.OdinInspector;

namespace Castle.Core.TimeTools
{
    [System.Serializable,InlineProperty]
    public struct DateRange : IConditionalCastleRange<System.DateTime>
    {
        [HorizontalGroup,HideLabel]
        public SimpleDate from, to;
        
        public bool Check(System.DateTime variable)
        {
            return false;
        }

        public string Label { get; }
    }
}