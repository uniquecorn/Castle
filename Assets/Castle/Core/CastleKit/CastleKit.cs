using System;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Castle.Core.Save
{
    public static class CastleKit
    {
        const string Lib = "__Internal";
        //public static async UniTask<bool> Login() => (await GKLocalPlayer.Authenticate()).IsAuthenticated;
        [DllImport(Lib)]
        private static extern void _GKAuthenticate(IntPtr callback, IntPtr didModifySavedGameCallback, IntPtr hasConflictingSavedGamesCallback);
        
        public static void AuthenticateLocalPlayer(Action<CastleResult> callback,Action<CastleResult> modifiedSaves,Action<CastleResult> conflict)
        {
#if UNITY_IOS
            _GKAuthenticate(
                MonoPCallback.ActionToIntPtr(callback),
                MonoPCallback.ActionToIntPtr(modifiedSaves),
                MonoPCallback.ActionToIntPtr(conflict)
            );
#endif
        }
    }
}
