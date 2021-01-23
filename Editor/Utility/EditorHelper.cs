#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;

namespace FrameworkEditor
{
    public static class EditorHelper
    {
        public static string GetCurrentDirectory()
        {
            var flag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
            var asm = Assembly.Load("UnityEditor.dll");
            var typeProjectBrowser = asm.GetType("UnityEditor.ProjectBrowser");
            var projectBrowserWindow = EditorWindow.GetWindow(typeProjectBrowser);
            return (string)typeProjectBrowser.GetMethod("GetActiveFolderPath", flag)?.Invoke(projectBrowserWindow, null); 
        }
    }
}
#endif