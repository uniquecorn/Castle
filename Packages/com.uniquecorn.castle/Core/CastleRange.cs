using Sirenix.OdinInspector;

namespace Castle.Core
{
    [System.Serializable]
    public abstract class CastleRange : ISimpleCastleRange
    {
        public abstract string Label { get; }
        [Title("$Label"), ShowInInspector, ShowIf("UseRangeEnum"), PropertyOrder(-3), HideReferenceObjectPicker,
         InlineButton("DebugCheck", ShowIf = "@Application.isPlaying")]
        protected virtual System.Enum RangeType
        {
            get => null;
            set => _ = value;
        }
        private bool UseRangeEnum => RangeType != null;
        public virtual bool Check() => false;
    }
}