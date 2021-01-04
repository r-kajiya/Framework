using System;

#if USE_FIREBASE

#endif

namespace Framework
{
    public static class Network
    {
#if USE_FIREBASE
        public static bool DidAnonymouslyLoggedIn => MFBFNetwork.DidAnonymouslyLoggedIn;
#endif        
        public static void SignInAnonymously(Action<string> onSignInRegister)
        {
#if USE_FIREBASE
            MFBFNetwork.SignInAnonymously(onSignInRegister);
#endif
        }

        public static void SetById(string key, string id, string json, Action onSuccess, Action onFailed)
        {
#if USE_FIREBASE
            MFBFNetwork.SetById(key, id, json, onSuccess, onFailed);
#endif            
        }
        
        public static void GetOrderByFirst(string key, string sortKey, int fetchCount, Action<string> onSuccess, Action onFailed)
        {
#if USE_FIREBASE
            MFBFNetwork.GetOrderByFirst(key, sortKey, fetchCount, onSuccess, onFailed);
#endif
        }
        
        public static void GetOrderByLast(string key, string sortKey, int fetchCount, Action<string> onSuccess, Action onFailed)
        {
#if USE_FIREBASE
            MFBFNetwork.GetOrderByLast(key, sortKey, fetchCount, onSuccess, onFailed);
#endif
        }
    }
}
