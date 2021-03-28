#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FrameworkEditor
{
    public static class Screenshot
    {
        const string ITEM_NAME = "Framework/Take Screenshot";

        [MenuItem(ITEM_NAME)]
        public static void Take()
        {
            var assetDir = new DirectoryInfo(Application.dataPath + "/Assets");
            var screenshotsDir = assetDir.Parent.CreateSubdirectory("Screenshots");
            var file = $"{System.DateTime.Now:yyyyMMddHHmmss}.png";
            var path = Path.Combine(screenshotsDir.FullName, file);
            
            ScreenCapture.CaptureScreenshot(path);
            AssetDatabase.Refresh();

            Debug.Log($"Saved screenshot: {path}");
        }
    }
}

#endif