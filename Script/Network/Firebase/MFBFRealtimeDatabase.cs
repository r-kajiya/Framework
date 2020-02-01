#if USE_FIREBASE
using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
#if UNITY_EDITOR
using Firebase.Unity.Editor;
#endif

namespace Framework
{
    public static class FirebaseRealtimeDatabase
    {
        public static void SetupDatabase(string firebaseUrl)
        {
#if UNITY_EDITOR
            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(firebaseUrl);
#endif
        }

        public static void GetByID(string key, string id, Action<string> onSuccess, Action onFailed)
        {
            DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
            DatabaseReference databaseReferenceKey = databaseReference.Child(key);
            DatabaseReference databaseReferenceId = databaseReferenceKey.Child(id);
            Task<DataSnapshot> task = databaseReferenceId.GetValueAsync();

            task.ContinueWith(snapshotResultTask =>
            {
                if (snapshotResultTask.IsCanceled)
                {
                    DebugLog.Normal($"Firebase RealtimeDatabase : Canceled データの取得に失敗しました : DB:{key}");
                    onFailed?.Invoke();
                }
                else if (snapshotResultTask.IsFaulted)
                {
                    DebugLog.Normal($"Firebase RealtimeDatabase : Faulted データの取得に失敗しました : DB:{key}");
                    onFailed?.Invoke();
                }
                else if (snapshotResultTask.IsCompleted)
                {
                    string json = snapshotResultTask.Result.GetRawJsonValue();
                    DebugLog.Normal($"Firebase RealtimeDatabase : Completed データを取得しました : DB:{key} Json:{json}");
                    onSuccess(json);
                }
            });
        }
        
        public static void GetAll(string key, Action<string> onSuccess, Action onFailed)
        {
            DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
            DatabaseReference databaseReferenceKey = databaseReference.Child(key);
            Task<DataSnapshot> task = databaseReferenceKey.GetValueAsync();

            task.ContinueWith(snapshotResultTask =>
            {
                if (snapshotResultTask.IsCanceled)
                {
                    DebugLog.Normal($"Firebase RealtimeDatabase : Canceled データの全取得に失敗しました : DB:{key}");
                    onFailed?.Invoke();
                }
                else if (snapshotResultTask.IsFaulted)
                {
                    DebugLog.Normal($"Firebase RealtimeDatabase : Faulted データの全取得に失敗しました : DB:{key}");
                    onFailed?.Invoke();
                }
                else if (snapshotResultTask.IsCompleted)
                {
                    string json = task.Result.GetRawJsonValue();
                    DebugLog.Normal($"Firebase RealtimeDatabase : Completed データを全取得しました : DB:{key} Json:{json}");
                    onSuccess(json);
                }
            });
        }
        
        public static void GetOrderByFirst(string key, string sortKey, int fetchCount, Action<string> onSuccess, Action onFailed)
        {
            DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
            DatabaseReference databaseReferenceKey = databaseReference.Child(key);
            Task<DataSnapshot> task = databaseReferenceKey.OrderByChild(sortKey).LimitToFirst(fetchCount).GetValueAsync();

            task.ContinueWith(snapshotResultTask =>
            {
                if (snapshotResultTask.IsCanceled)
                {
                    DebugLog.Normal($"Firebase RealtimeDatabase : Canceled データの降順取得に失敗しました : DB:{key}");
                    onFailed?.Invoke();
                }
                else if (snapshotResultTask.IsFaulted)
                {
                    DebugLog.Normal($"Firebase RealtimeDatabase : Faulted データの降順取得に失敗しました : DB:{key}");
                    onFailed?.Invoke();
                }
                else if (snapshotResultTask.IsCompleted)
                {
                    string json = snapshotResultTask.Result.GetRawJsonValue();
                    DebugLog.Normal($"Firebase RealtimeDatabase : Completed データを降順取得しました : DB:{key} Json:{json}");
                    onSuccess(json);
                }
            });
        }
        
        public static void GetOrderByLast(string key, string sortKey, int fetchCount, Action<string> onSuccess, Action onFailed)
        {
            DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
            DatabaseReference databaseReferenceKey = databaseReference.Child(key);
            Task<DataSnapshot> task = databaseReferenceKey.OrderByChild(sortKey).LimitToLast(fetchCount).GetValueAsync();

            task.ContinueWith(snapshotResultTask =>
            {
                if (snapshotResultTask.IsCanceled)
                {
                    DebugLog.Normal($"Firebase RealtimeDatabase : Canceled データの昇順取得に失敗しました : DB:{key}");
                    onFailed?.Invoke();
                }
                else if (snapshotResultTask.IsFaulted)
                {
                    DebugLog.Normal($"Firebase RealtimeDatabase : Faulted データの昇順取得に失敗しました : DB:{key}");
                    onFailed?.Invoke();
                }
                else if (snapshotResultTask.IsCompleted)
                {
                    string json = snapshotResultTask.Result.GetRawJsonValue();
                    DebugLog.Normal($"Firebase RealtimeDatabase : Completed データを昇順取得しました : DB:{key} Json:{json}");
                    onSuccess(json);
                }
            });
        }

        public static void SetByID(string key, string id, string json, Action onSuccess, Action onFailed)
        {
            DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
            DatabaseReference databaseReferenceKey = databaseReference.Child(key);
            DatabaseReference databaseReferenceId = databaseReferenceKey.Child(id);
            Task task = databaseReferenceId.SetRawJsonValueAsync(json);

            task.ContinueWith(resultTask =>
            {
                if (resultTask.IsCanceled)
                {
                    DebugLog.Normal($"Firebase RealtimeDatabase : Canceled データの保存に失敗しました : DB:{key}");
                    onFailed?.Invoke();
                }
                else if (resultTask.IsFaulted)
                {
                    DebugLog.Normal($"Firebase RealtimeDatabase : Faulted データの保存に失敗しました : DB:{key}");
                    onFailed?.Invoke();
                }
                else if (resultTask.IsCompleted)
                {
                    DebugLog.Normal($"Firebase RealtimeDatabase : Completed データを保存しました : DB:{key} Json:{json}");
                    onSuccess?.Invoke();
                }
            });
        }
    }    
}
#endif