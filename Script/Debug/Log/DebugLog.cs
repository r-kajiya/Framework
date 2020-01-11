using UnityEngine;
using System.Diagnostics;

namespace Framework
{
    public static class DebugLog
    {
        static DebugCanvas _canvas;
        [Conditional("DEBUG"), RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InitializeLog()
        {
            var prefab = Resources.Load<DebugCanvas>("Debug/DebugCanvas");
            _canvas = GameObject.Instantiate<DebugCanvas>(prefab);
            Application.logMessageReceived += UnityLogHanlder;
        }

        static void UnityLogHanlder(string log, string stackTrace, LogType type)
        {
            switch(type)
            {
                case LogType.Log:
                    _canvas?.Log(log);
                    break;
                case LogType.Warning:
                    _canvas?.LogWarning(log);
                    break;
                case LogType.Error:
                case LogType.Exception:
                case LogType.Assert:
                    _canvas?.LogError(log);
                    break;
            }
        }

        [Conditional("DEBUG")]
        public static void Normal(string log)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log(log);
#else
            _canvas?.Log(log);
#endif
        }

        [Conditional("DEBUG")]
        public static void Warning(string log)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning(log);
#else
            _canvas?.LogWarning(log);
#endif
        }

        [Conditional("DEBUG")]
        public static void Error(string log)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogError(log);
#else
            _canvas?.LogError(log);
#endif
        }
    }
}

