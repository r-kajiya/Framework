#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    public class GenerateCleanArchitectureCodeEditorWindow : EditorWindow
    {
        const string ITEM_NAME = "Framework/CleanArchitecture Generate Window";
        const string WINDOW_TITLE = "CleanArchitecture Generate Window";
        
        TextAsset _textAsset;
        string _namespace;
        string _path;

        [MenuItem(ITEM_NAME)]
        static void Open()
        {
            GetWindow<GenerateCleanArchitectureCodeEditorWindow>(true, WINDOW_TITLE);
        }

        void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            _textAsset = (TextAsset) EditorGUILayout.ObjectField ("スキーマ定義json", _textAsset, typeof(TextAsset), false);
            EditorGUILayout.EndHorizontal();
            _path = AssetDatabase.GetAssetPath (_textAsset);

            _namespace = EditorGUILayout.TextField("namespace", _namespace);
            
            if (GUILayout.Button("コード生成"))
            {
                if (Validate())
                {
                    Execute();
                }
            }
        }

        bool Validate()
        {
            FileInfo info = new FileInfo(_path);
            
            if (!info.Extension.Contains("json"))
            {
                Debug.LogError("jsonファイルではありません");
                return false;
            }
            
            if (!info.Name.Contains("schema"))
            {
                Debug.LogError("定義ファイルではありません");
                return false;
            }

            if (string.IsNullOrEmpty(_namespace))
            {
                Debug.LogError("namespaceを入力してください");
                return false;
            }

            return true;
        }
        
        void Execute()
        {
            GenerateUseCase();
        }

        void GenerateUseCase()
        {
            // StringBuilder stringBuilder = new StringBuilder();
            // stringBuilder.AppendLine("using System;");
            // stringBuilder.AppendLine("using UnityEngine;");
            // stringBuilder.AppendLine("using Framework;");
            // stringBuilder.AppendLine();
            // stringBuilder.Append("namespace ").AppendLine(_namespace);
            // stringBuilder.AppendLine("{");
            // stringBuilder.AppendLine("    [Serializable]");
            // stringBuilder.Append("    public class ").Append(_entityName).AppendLine(" : IEntity");
            // stringBuilder.AppendLine("    {");
            // foreach (var pair in _keyTypeMap)
            // {
            //     stringBuilder.AppendLine("        [SerializeField, HideInInspector]");
            //
            //     if (pair.Value.Contains("int"))
            //     {
            //         stringBuilder.Append("        int ").Append(pair.Key).AppendLine(" = -1;");
            //         stringBuilder.Append("        public int ").Append(pair.Key.ToTitleUpperCase()).Append("=> ").Append(pair.Key).AppendLine(";");
            //     }
            //     else if (pair.Value.Contains("float"))
            //     {
            //         stringBuilder.Append("        float ").Append(pair.Key).AppendLine(" = -1.0f;");
            //         stringBuilder.Append("        public float ").Append(pair.Key.ToTitleUpperCase()).Append("=> ").Append(pair.Key).AppendLine(";");
            //     }
            //     else if (pair.Value.Contains("string"))
            //     {
            //         stringBuilder.Append("        string ").Append(pair.Key).AppendLine(" = \"\";");
            //         stringBuilder.Append("        public string ").Append(pair.Key.ToTitleUpperCase()).Append("=> ").Append(pair.Key).AppendLine(";");
            //     }
            //
            //     stringBuilder.AppendLine();
            // }
            // stringBuilder.Append("        public ").Append(_entityName).Append("(").Append(_modelName).AppendLine(" model)");
            // stringBuilder.AppendLine("        {");
            // foreach (var pair in _keyTypeMap)
            // {
            //     stringBuilder.Append("            ").Append(pair.Key).Append(" = model.").Append(pair.Key.ToTitleUpperCase()).AppendLine(";");
            // }
            // stringBuilder.AppendLine("        }");
            // stringBuilder.AppendLine("    }");
            // stringBuilder.AppendLine("}");
            //
            // string fileName = _entityName + ".cs";
            // FileInfo info = new FileInfo(_path);
            // string path = info.DirectoryName + "/" + fileName;
            //
            // FileUtil.SaveText(stringBuilder.ToString(), path);
            //
            // Debug.Log(path+"に生成しました");
        }
    }    
}

#endif