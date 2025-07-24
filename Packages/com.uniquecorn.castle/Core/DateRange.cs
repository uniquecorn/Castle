using System;
using Sirenix.OdinInspector;

namespace Castle.Core
{
    [Serializable,InlineProperty]
    public struct DateRange : IConditionalCastleRange<DateTime>
    {
        [HorizontalGroup,HideLabel]
        public SimpleDate from, to;
        
        public bool Check(DateTime variable)
        {
            return false;
        }

        public string Label { get; }
    }
}