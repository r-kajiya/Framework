#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using Framework;
using UnityEditor;
using UnityEngine;

namespace FrameworkEditor
{
    public class GenerateMasterDataCodeEditorWindow : EditorWindow
    {
        const string ITEM_NAME = "Framework/MasterDataCode Generate Window";
        const string WINDOW_TITLE = "MasterDataCode Generate Window";
        
        
        TextAsset _textAsset;
        string _namespace;
        string _path;
        string _masterName;
        string _modelName;
        string _entityName;
        string _datastoreName;
        string _primaryKeyName;
        string _repositoryName;
        Dictionary<string, string> _keyTypeMap = new Dictionary<string, string>();
        
        [MenuItem(ITEM_NAME)]
        static void Open()
        {
            GetWindow<GenerateMasterDataCodeEditorWindow>(true, WINDOW_TITLE);
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
            _keyTypeMap.Clear();
            
            FileInfo info = new FileInfo(_path);
            StreamReader reader = new StreamReader(info.OpenRead());

            _masterName = info.Name.Replace("_schema.json", "");
            _modelName = _masterName + "Model";
            _entityName = _masterName + "Entity";;
            _datastoreName = _masterName + "DataStore";;
            _primaryKeyName = _masterName + "PrimaryKey";;
            _repositoryName = _masterName + "Repository";;
            
            while (reader.Peek() != -1)
            {
                string line = reader.ReadLine();

                if (line != null && (line.Contains("[") ||
                                     line.Contains("]") ||
                                     line.Contains("}") ||
                                     line.Contains("{")))
                {
                    continue;
                }

                line = line.Replace("\"", "");
                line = line.Replace(",", "");
                line = line.Replace(" ", "");
                string[] temp =  line.Split(':');
                string key = temp[0];
                string type = temp[1];
                _keyTypeMap[key] = type;
            }

            reader.Close();

            GenerateEntity();
            GenerateModel();
            GeneratePrimaryKey();
            GenerateDataStore();
            GenerateRepository();
        }

        void GenerateEntity()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("using System;");
            stringBuilder.AppendLine("using UnityEngine;");
            stringBuilder.AppendLine("using Framework;");
            stringBuilder.AppendLine();
            stringBuilder.Append("namespace ").AppendLine(_namespace);
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("    [Serializable]");
            stringBuilder.Append("    public class ").Append(_entityName).AppendLine(" : IEntity");
            stringBuilder.AppendLine("    {");
            foreach (var pair in _keyTypeMap)
            {
                stringBuilder.AppendLine("        [SerializeField, HideInInspector]");

                if (pair.Value.Contains("int"))
                {
                    stringBuilder.Append("        int ").Append(pair.Key).AppendLine(" = -1;");
                    stringBuilder.Append("        public int ").Append(pair.Key.ToTitleUpperCase()).Append("=> ").Append(pair.Key).AppendLine(";");
                }
                else if (pair.Value.Contains("float"))
                {
                    stringBuilder.Append("        float ").Append(pair.Key).AppendLine(" = -1.0f;");
                    stringBuilder.Append("        public float ").Append(pair.Key.ToTitleUpperCase()).Append("=> ").Append(pair.Key).AppendLine(";");
                }
                else if (pair.Value.Contains("string"))
                {
                    stringBuilder.Append("        string ").Append(pair.Key).AppendLine(" = \"\";");
                    stringBuilder.Append("        public string ").Append(pair.Key.ToTitleUpperCase()).Append("=> ").Append(pair.Key).AppendLine(";");
                }

                stringBuilder.AppendLine();
            }
            stringBuilder.Append("        public ").Append(_entityName).Append("(").Append(_modelName).AppendLine(" model)");
            stringBuilder.AppendLine("        {");
            foreach (var pair in _keyTypeMap)
            {
                stringBuilder.Append("            ").Append(pair.Key).Append(" = model.").Append(pair.Key.ToTitleUpperCase()).AppendLine(";");
            }
            stringBuilder.AppendLine("        }");
            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("}");

            string fileName = _entityName + ".cs";
            FileInfo info = new FileInfo(_path);
            string path = info.DirectoryName + "/" + fileName;

            FileUtil.SaveText(stringBuilder.ToString(), path);
            
            Debug.Log(path+"に生成しました");
        }

        void GenerateModel()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("using Framework;");
            stringBuilder.AppendLine();
            stringBuilder.Append("namespace ").AppendLine(_namespace);
            stringBuilder.AppendLine("{");
            stringBuilder.Append("    public class ").Append(_modelName).AppendLine(" : ModelBase");
            stringBuilder.AppendLine("    {");
            
            foreach (var pair in _keyTypeMap)
            {
                stringBuilder.Append("        public ").Append(pair.Value).Append(" ").Append(pair.Key.ToTitleUpperCase()).AppendLine(" { get; }");
                stringBuilder.AppendLine();
            }

            stringBuilder.Append("        public ").Append(_modelName).Append("(");
            int i = 0; 
            foreach (var pair in _keyTypeMap)
            {
                if (_keyTypeMap.Count - 1 <= i)
                {
                    stringBuilder.Append(pair.Value).Append(" ").Append(pair.Key).Append(")");
                }
                else
                {
                    stringBuilder.Append(pair.Value).Append(" ").Append(pair.Key).Append(",");
                }
                i++;
            }
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("        {");
            foreach (var pair in _keyTypeMap)
            {
                stringBuilder.Append("            ").Append(pair.Key.ToTitleUpperCase()).Append(" = ").Append(pair.Key).AppendLine(";");
            }
            stringBuilder.AppendLine("        }");
            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("}");

            string fileName = _modelName + ".cs";
            FileInfo info = new FileInfo(_path);
            string path = info.DirectoryName + "/" + fileName;

            FileUtil.SaveText(stringBuilder.ToString(), path);
            
            Debug.Log(path+"に生成しました");
        }

        void GeneratePrimaryKey()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("using Framework;");
            stringBuilder.AppendLine();
            stringBuilder.Append("namespace ").AppendLine(_namespace);
            stringBuilder.AppendLine("{");
            stringBuilder.Append("    public class ").Append(_masterName).Append("PrimaryKey : IPrimaryKey<").Append(_primaryKeyName).Append(", ").Append(_modelName).AppendLine(">");
            stringBuilder.AppendLine("    {");
            stringBuilder.Append("        ").Append(_modelName).AppendLine(" _model;");
            stringBuilder.AppendLine();
            stringBuilder.Append("        public void Setup(").Append(_modelName).AppendLine(" model)");
            stringBuilder.AppendLine("        {");
            stringBuilder.AppendLine("            _model = model;");
            stringBuilder.AppendLine("        }");
            stringBuilder.AppendLine();
            stringBuilder.Append("        public bool Equals(").Append(_primaryKeyName).AppendLine(" other)");
            stringBuilder.AppendLine("        {");
            stringBuilder.AppendLine("            if (ReferenceEquals(null, other))");
            stringBuilder.AppendLine("            {");
            stringBuilder.AppendLine("                return false;");
            stringBuilder.AppendLine("            }");
            stringBuilder.AppendLine("            if (ReferenceEquals(this, other))");
            stringBuilder.AppendLine("            {");
            stringBuilder.AppendLine("                return true;");
            stringBuilder.AppendLine("            }");
            stringBuilder.AppendLine("            // TODO: 比較式を記入してください");
            stringBuilder.AppendLine("            return true;");
            stringBuilder.AppendLine("        }");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("        public override bool Equals(object obj)");
            stringBuilder.AppendLine("        {");
            stringBuilder.AppendLine("            if (ReferenceEquals(null, obj))");
            stringBuilder.AppendLine("            {");
            stringBuilder.AppendLine("                return false;");
            stringBuilder.AppendLine("            }");
            stringBuilder.AppendLine("            if (ReferenceEquals(this, obj))");
            stringBuilder.AppendLine("            {");
            stringBuilder.AppendLine("                return true;");
            stringBuilder.AppendLine("            }");
            stringBuilder.AppendLine("            if (obj.GetType() != this.GetType())");
            stringBuilder.AppendLine("            {");
            stringBuilder.AppendLine("                return false;");
            stringBuilder.AppendLine("            }");
            stringBuilder.AppendLine("            ");
            stringBuilder.Append("            return Equals((").Append(_primaryKeyName).AppendLine(") obj);");
            stringBuilder.AppendLine("        }");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("        public override int GetHashCode()");
            stringBuilder.AppendLine("        {");
            stringBuilder.AppendLine("            return _model != null ? _model.GetHashCode() : 0;");
            stringBuilder.AppendLine("        }");
            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("}");

            string fileName = _primaryKeyName + ".cs";
            FileInfo info = new FileInfo(_path);
            string path = info.DirectoryName + "/" + fileName;

            FileUtil.SaveText(stringBuilder.ToString(), path);
            
            Debug.Log(path+"に生成しました");
        }
        
        void GenerateDataStore()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("using System;");
            stringBuilder.AppendLine("using System.IO;");
            stringBuilder.AppendLine("using System.Collections.Generic;");
            stringBuilder.AppendLine("using UnityEngine;");
            stringBuilder.AppendLine("using Framework;");
            stringBuilder.AppendLine();
            stringBuilder.Append("namespace ").AppendLine(_namespace);
            stringBuilder.AppendLine("{");
            stringBuilder.Append("    public class ").Append(_datastoreName).Append(" : IDataStore<").Append(_modelName).Append(", ").Append(_primaryKeyName).AppendLine(">");
            stringBuilder.AppendLine("    {");
            stringBuilder.Append("        const string FILE_PATH = \"Master/").Append(_masterName).AppendLine(".json\";");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("        [Serializable]");
            stringBuilder.AppendLine("        class Entities");
            stringBuilder.AppendLine("        {");
            stringBuilder.Append("            public ").Append(_entityName).AppendLine("[] Values = null;");
            stringBuilder.AppendLine("        }");
            stringBuilder.AppendLine();
            stringBuilder.Append("        public void Save(").Append(_modelName).AppendLine(" model) { }");
            stringBuilder.Append("        public void SaveList(List<").Append(_modelName).AppendLine("> models) { }");
            stringBuilder.AppendLine();
            stringBuilder.Append("       public Dictionary<").Append(_primaryKeyName).Append(", ").Append(_modelName).AppendLine("> Load()");
            stringBuilder.AppendLine("        {");
            stringBuilder.AppendLine("            string assetsPath = $\"{Application.streamingAssetsPath}/{FILE_PATH}\";");
            stringBuilder.AppendLine("            string json = \"\";");
            stringBuilder.AppendLine("#if UNITY_ANDROID && !UNITY_EDITOR");
            stringBuilder.AppendLine("            WWW www = new WWW(assetsPath);");
            stringBuilder.AppendLine("            while (!www.isDone) { }");
            stringBuilder.AppendLine("            string txtBuffer = string.Empty;");
            stringBuilder.AppendLine("            TextReader txtReader = new StringReader(www.text);");
            stringBuilder.AppendLine("            string description = string.Empty;");
            stringBuilder.AppendLine("            while ((txtBuffer = txtReader.ReadLine()) != null)");
            stringBuilder.AppendLine("            {");
            stringBuilder.AppendLine("                description = description + txtBuffer;");
            stringBuilder.AppendLine("            }");
            stringBuilder.AppendLine("            json = description;");
            stringBuilder.AppendLine("#else");
            stringBuilder.AppendLine("            FileInfo info = new FileInfo(assetsPath);");
            stringBuilder.AppendLine("            StreamReader reader = new StreamReader(info.OpenRead());");
            stringBuilder.AppendLine("            json = reader.ReadToEnd();");
            stringBuilder.AppendLine("            reader.Close();");
            stringBuilder.AppendLine("#endif");
            stringBuilder.AppendLine("            if (string.IsNullOrEmpty(json))");
            stringBuilder.AppendLine("            {");
            stringBuilder.AppendLine("                DebugLog.Warning(FILE_PATH + \"が存在していますがファイルの中身がありません\");");
            stringBuilder.Append("                return new Dictionary<").Append(_primaryKeyName).Append(", ").Append(_modelName).AppendLine(">();");
            stringBuilder.AppendLine("            }");
            stringBuilder.AppendLine();
            stringBuilder.Append("            var map = new Dictionary<").Append(_primaryKeyName).Append(", ").Append(_modelName).AppendLine(">();");
            stringBuilder.AppendLine("            var parse = JsonUtility.FromJson<Entities>(json);");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("            foreach (var entity in parse.Values)");
            stringBuilder.AppendLine("            {");
            stringBuilder.Append("                ").Append(_modelName).Append(" model = new ").Append(_modelName).AppendLine("(");
            int i = 0;
            foreach (var pair in _keyTypeMap)
            {
                if (_keyTypeMap.Count - 1 <= i)
                {
                    stringBuilder.Append("                    entity.").Append(pair.Key.ToTitleUpperCase()).AppendLine(");");   
                }
                else
                {
                    stringBuilder.Append("                    entity.").Append(pair.Key.ToTitleUpperCase()).AppendLine(",");
                }

                i++;
            }
            stringBuilder.AppendLine();
            stringBuilder.Append("                ").Append(_primaryKeyName).Append(" primaryKey = new ").Append(_primaryKeyName).AppendLine("();");
            stringBuilder.AppendLine("                primaryKey.Setup(model);");
            stringBuilder.AppendLine("                map.Add(primaryKey, model);");
            stringBuilder.AppendLine("            }");
            stringBuilder.AppendLine("            return map;");
            stringBuilder.AppendLine("        }");
            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("}");

            string fileName = _datastoreName + ".cs";
            FileInfo info = new FileInfo(_path);
            string path = info.DirectoryName + "/" + fileName;

            FileUtil.SaveText(stringBuilder.ToString(), path);
            
            Debug.Log(path+"に生成しました");
        }

        void GenerateRepository()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("using Framework;");
            stringBuilder.AppendLine();
            stringBuilder.Append("namespace ").AppendLine(_namespace);
            stringBuilder.AppendLine("{");
            stringBuilder.Append("    public class ").Append(_repositoryName).Append(" : BaseRepository<").Append(_modelName).Append(", ").Append(_datastoreName).Append(", ").Append(_primaryKeyName).AppendLine(">");
            stringBuilder.AppendLine("    {");
            stringBuilder.Append("        public static ").Append(_repositoryName).AppendLine(" I { get; }");
            stringBuilder.AppendLine();
            stringBuilder.Append("        static ").Append(_repositoryName).AppendLine("()");
            stringBuilder.AppendLine("        {");
            stringBuilder.Append("            I = new ").Append(_repositoryName).AppendLine("();");
            stringBuilder.AppendLine("        }");
            stringBuilder.AppendLine();
            stringBuilder.Append("        ").Append(_repositoryName).Append("() : base(new ").Append(_datastoreName).AppendLine("()) {}");
            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("}");
            
            string fileName = _repositoryName + ".cs";
            FileInfo info = new FileInfo(_path);
            string path = info.DirectoryName + "/" + fileName;

            FileUtil.SaveText(stringBuilder.ToString(), path);
            
            Debug.Log(path+"に生成しました");
        }
    }    
}

#endif