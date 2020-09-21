#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    public static class FileUtil
    {
        public static bool SaveText( string text, string path)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }
            
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            
            var streamWriter = new StreamWriter(path);
            streamWriter.WriteLine(text);
            streamWriter.Flush();
            streamWriter.Close();
            
            if (path.Contains(Application.dataPath))
            {
                AssetDatabase.Refresh();
            }

            return true;
        }
    }
}

#endif