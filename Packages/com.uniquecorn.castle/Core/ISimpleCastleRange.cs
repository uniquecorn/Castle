using UnityEngine;

namespace Castle.Core
{
    public interface ISimpleCastleRange : ICastleRange
    {
        bool Check();
        void DebugCheck() => Debug.Log(Label +" is "+Check());
    }
}