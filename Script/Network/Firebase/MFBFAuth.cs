#if USE_FIREBASE
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System;


namespace Framework
{
    public static class MFBFAuth
    {
        public static void Login(string userId, Action<string> onSignIn)
        {
            DebugLog.Normal("FireBase Auth : ログイン認証を行います");
            
            if (string.IsNullOrEmpty(userId))
            {
                SignInAnonymously(user =>
                {
                    onSignIn?.Invoke(user.UserId);
                });
            }
            else
            {
                onSignIn?.Invoke(userId);
            }
        }

        static void SignInAnonymously(Action<FirebaseUser> onResult)
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
                
                FirebaseUser newUser = task.Result;
                DebugLog.Normal($"FireBase Auth : 匿名ユーザーを作成しました {newUser.DisplayName} ({newUser.UserId})");
                
                onResult?.Invoke(newUser);
            });
        }
    }    
}
#endif