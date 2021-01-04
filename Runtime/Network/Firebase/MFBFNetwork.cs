#if USE_FIREBASE
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System;


namespace Framework
{
    public static class MFBFNetwork
    {
        const string DATA_BASE_URL = "https://super-suport-race.firebaseio.com/";
        public static bool DidAnonymouslyLoggedIn => MFBFAuth.DidAnonymouslyLoggedIn;

        public static void SignInAnonymously(Action<string> onSignIn)
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
                
                DependencyStatus dependencyStatus = task.Result;
                
                if (dependencyStatus == DependencyStatus.Available)
                {
                    DebugLog.Normal("Firebase : ユーザー設定を行います");
                    MFBFAuth.SignInAnonymously(onSignIn);
                } 
                else
                {
                    DebugLog.Error("Firebase : 依存関係を解決できませんでした: " + dependencyStatus);
                }
                
                MFBFDatabase.SetupDatabase(DATA_BASE_URL);
            });
        }

        public static void SetupAnalytics(string userId)
        {
            MFBFAnalytics.Setup(userId);
            MFBFAnalytics.Login(userId);
        }

        public static void GetById(string key, string id, Action<string> onSuccess, Action onFailed)
        {
            MFBFDatabase.GetByID(key, id, onSuccess, onFailed);
        }
        
        public static void GetAll(string key, Action<string> onSuccess, Action onFailed)
        {
            MFBFDatabase.GetAll(key, onSuccess, onFailed);
        }
        
        public static void GetOrderByFirst(string key, string sortKey, int fetchCount, Action<string> onSuccess, Action onFailed)
        {
            MFBFDatabase.GetOrderByFirst(key, sortKey, fetchCount, onSuccess, onFailed);
        }
        
        public static void GetOrderByLast(string key, string sortKey, int fetchCount, Action<string> onSuccess, Action onFailed)
        {
            MFBFDatabase.GetOrderByLast(key, sortKey, fetchCount, onSuccess, onFailed);
        }
        
        public static void SetById(string key, string id, string json, Action onSuccess, Action onFailed)
        {
            MFBFDatabase.SetByID(key, id, json, onSuccess, onFailed);
        }
    }    
}
#endif