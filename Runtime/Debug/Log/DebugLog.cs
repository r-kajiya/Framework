using UnityEngine;
using System.Diagnostics;

namespace Framework
{
    public static class DebugLog
    {
        static DebugCanvas _canvas = null;
        
        [Conditional("DEBUG"), RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InitializeLog()
        {
#if USE_DEBUG_LOG
            var prefab = Resources.Load<DebugCanvas>("Debug/DebugCanvas");
            _canvas = Object.Instantiate<DebugCanvas>(prefab);
            Application.logMessageReceived += UnityLogHandler;
#endif
        }

        static void UnityLogHandler(string log, string stackTrace, LogType type)
        {
            switch(type)
            {
                case LogType.Log:
                    _canvas.Log(log);
                    break;
                case LogType.Warning:
                    _canvas.LogWarning(log);
                    break;
                case LogType.Error:
                case LogType.Exception:
                case LogType.Assert:
                    _canvas.LogError(log);
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
        public static void Normal(string log, Color color)
        {
            string colorLog = $"<color=#{color.ColorToHex()}>{log}</color>";
#if UNITY_EDITOR
            UnityEngine.Debug.Log(colorLog);
#else
            _canvas?.Log(colorLog);
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
        public static void Warning(string log, Color color)
        {
            string colorLog = $"<color=#{color.ColorToHex()}>{log}</color>";
#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning(colorLog);
#else
            _canvas?.LogWarning(colorLog);
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
        
        [Conditional("DEBUG")]
        public static void Error(string log, Color color)
        {
            string colorLog = $"<color=#{color.ColorToHex()}>{log}</color>";
#if UNITY_EDITOR
            UnityEngine.Debug.LogError(colorLog);
#else
            _canvas?.LogError(colorLog);
#endif
        }
    }
}