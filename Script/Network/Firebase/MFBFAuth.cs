#if USE_FIREBASE
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System;


namespace Framework
{
    public static class MFBFAuth
    {
        public static bool DidAnonymouslyLoggedIn { get; private set; } = false;
        
        public static void SignInAnonymously(Action<string> onResult)
        {
            DebugLog.Normal($"FireBase Auth : 匿名ログインを行います");

            FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    DebugLog.Error("FireBase Auth : 匿名ログインをキャンセルしました");
                    return;
                }
                
                if (task.IsFaulted)
                {
                    DebugLog.Error("FireBase Auth : 匿名ログインに失敗しました。" + task.Exception);
                    return;
                }
                
                FirebaseUser anonymousUser = task.Result;
                DebugLog.Normal($"FireBase Auth : 匿名ユーザーを作成しました {anonymousUser.DisplayName} ({anonymousUser.UserId})");
                DidAnonymouslyLoggedIn = true;
                
                onResult?.Invoke(anonymousUser.UserId);
            });
        }
    }    
}
#endif