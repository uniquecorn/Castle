using System.Threading;
using Castle.Core.Save;
using Cysharp.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

namespace Castle.Core
{
    public abstract class CastleGame : MonoBehaviour
    {
        public static CancellationTokenSource endGameToken;
        protected async UniTaskVoid StartUnityServices()
        {
            try {
#if UNITY_EDITOR
                var options = new InitializationOptions()
                    .SetEnvironmentName("dev");
#else
            var options = new InitializationOptions()
                .SetEnvironmentName("production");
#endif
                await UnityServices.InitializeAsync(options);
            }
            catch (System.Exception exception) {
                // An error occurred during initialization.
            }
        }
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
            StartUnityServices().Forget();
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