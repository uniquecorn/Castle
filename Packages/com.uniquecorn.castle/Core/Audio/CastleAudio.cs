using UnityEngine;

namespace Castle.Core.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class CastleAudio : MonoBehaviour
    {
        public new AudioSource audio;
        public AudioSource cross;
    }

    public abstract class BaseCastleAudio : CastleAudio
    {
        
    }
}
