using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Castle.Core
{
    public abstract class CastleGame : MonoBehaviour
    {
        public static CancellationTokenSource endGameToken;
    }
    public abstract class CastleGame<TGame, TSave> : CastleGame where TGame : CastleGame where TSave : CastleSave.Save<TSave>, new()
    {
        public TGame Instance;

        protected virtual void Awake()
        {
            if (this is TGame instance)
            {
                Instance = instance;
            }
            endGameToken = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
            Application.targetFrameRate = 60;
        }
        protected async UniTask<TSave> GetOrCreateSave(CancellationToken cts)
        {
            if (!await CastleSave.LoadGame<TSave>(cts))
            {
                return CastleSave.Save<TSave>.New();
            }
            return CastleSave.Save<TSave>.SaveInstance;
        }
    }
}