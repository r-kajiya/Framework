#if USE_FIREBASE
using Firebase.Analytics;
using System;


namespace Framework
{
    public static class MFBFAnalytics
    {
        public static void Setup(string userId)
        {
            DebugLog.Normal("Firebase Analytics : データ収集を有効化します");
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

#if UNITY_ANDROID
            FirebaseAnalytics.SetUserProperty(
                "OS",
                "Android");
#elif UNITY_IOS
            FirebaseAnalytics.SetUserProperty(
                "OS",
                "iOS");
#else
                FirebaseAnalytics.SetUserProperty(
                "OS",
                "Other");
#endif
            FirebaseAnalytics.SetSessionTimeoutDuration(new TimeSpan(0, 30, 0));
            FirebaseAnalytics.SetUserId(userId);
        }

        public static void Login(string userId)
        {
            DebugLog.Normal($"Firebase Analytics : ユーザーID : {userId} がログインしました");
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin);
        }
    }    
}
#endif