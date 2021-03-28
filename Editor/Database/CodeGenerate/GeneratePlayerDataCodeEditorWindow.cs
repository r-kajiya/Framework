#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using Framework;
using UnityEditor;
using UnityEngine;

namespace FrameworkEditor
{
    public class GeneratePlayerDataCodeEditorWindow : EditorWindow
    {
        const string ITEM_NAME = "Framework/プレイヤーデータ定義生成";
        const string WINDOW_TITLE = "PlayerDataCode Generate Window";

        TextAsset _textAsset;
        string _namespace;
        string _path;
        string _playerDataName;
        string _modelName;
        string _entityName;
        string _datastoreName;
        string _primaryKeyName;
        string _repositoryName;
        Dictionary<string, string> _keyTypeMap = new Dictionary<string, string>();
        bool _isGenerateModel = true;
        bool _isGenerateEntity = true;
        bool _isGenerateDataStore = true;
        bool _isGeneratePrimaryKey = true;
        bool _isGenerateRepository = true;

        [MenuItem(ITEM_NAME)]
        static void Open()
        {
            GetWindow<GeneratePlayerDataCodeEditorWindow>(true, WINDOW_TITLE);
        }

        void OnGUI()
        {
            _textAsset = (TextAsset) EditorGUILayout.ObjectField("スキーマ定義json", _textAsset, typeof(TextAsset), false);
            _path = AssetDatabase.GetAssetPath(_textAsset);
            _namespace = EditorGUILayout.TextField("namespace", _namespace);

            _isGenerateModel = EditorGUILayout.Toggle("Generate Model", _isGenerateModel);
            _isGenerateEntity = EditorGUILayout.Toggle("Generate Entity", _isGenerateEntity);
            _isGenerateDataStore = EditorGUILayout.Toggle("Generate DataStore", _isGenerateDataStore);
            _isGeneratePrimaryKey = EditorGUILayout.Toggle("Generate PrimaryKey", _isGeneratePrimaryKey);
            _isGenerateRepository = EditorGUILayout.Toggle("Generate Repository", _isGenerateRepository);

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

            if (!info.Name.Contains("Player"))
            {
                Debug.LogError("プレイヤー定義ファイルではありません");
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

            _playerDataName = info.Name.Replace("_schema.json", "");
            _modelName = _playerDataName + "Model";
            _entityName = _playerDataName + "Entity";
            _datastoreName = _playerDataName + "DataStore";
            _primaryKeyName = _playerDataName + "PrimaryKey";
            _repositoryName = _playerDataName + "Repository";

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
                string[] temp = line.Split(':');
                string key = temp[0];
                string type = temp[1];
                _keyTypeMap[key] = type;
            }

            reader.Close();

            if (_isGenerateModel)
            {
                GenerateModel();
            }

            if (_isGenerateEntity)
            {
                GenerateEntity();
            }

            if (_isGeneratePrimaryKey)
            {
                GeneratePrimaryKey();
            }

            if (_isGenerateDataStore)
            {
                GenerateDataStore();
            }

            if (_isGenerateRepository)
            {
                GenerateRepository();
            }
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
                    stringBuilder.Append("        public int ").Append(pair.Key.ToTitleUpperCase()).Append(" => ")
                        .Append(pair.Key).AppendLine(";");
                }
                else if (pair.Value.Contains("float"))
                {
                    stringBuilder.Append("        float ").Append(pair.Key).AppendLine(" = -1.0f;");
                    stringBuilder.Append("        public float ").Append(pair.Key.ToTitleUpperCase()).Append(" => ")
                        .Append(pair.Key).AppendLine(";");
                }
                else if (pair.Value.Contains("string"))
                {
                    stringBuilder.Append("        string ").Append(pair.Key).AppendLine(" = \"\";");
                    stringBuilder.Append("        public string ").Append(pair.Key.ToTitleUpperCase()).Append(" => ")
                        .Append(pair.Key).AppendLine(";");
                }
                else
                {
                    stringBuilder.Append("        " + pair.Value + " ").Append(pair.Key).AppendLine(";");
                    stringBuilder.Append("        public " + pair.Value + " ").Append(pair.Key.ToTitleUpperCase())
                        .Append(" => ").Append(pair.Key).AppendLine(";");
                }

                stringBuilder.AppendLine();
            }

            stringBuilder.Append("        public ").Append(_entityName).Append("(").Append(_modelName)
                .AppendLine(" model)");
            stringBuilder.AppendLine("        {");
            foreach (var pair in _keyTypeMap)
            {
                stringBuilder.Append("            ").Append(pair.Key).Append(" = model.")
                    .Append(pair.Key.ToTitleUpperCase()).AppendLine(";");
            }

            stringBuilder.AppendLine("        }");
            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("}");

            string projectPath = Directory.GetCurrentDirectory();
            string fileName = _entityName + ".cs";
            string[] filePathList = Directory.GetFiles(projectPath, "*.cs", System.IO.SearchOption.AllDirectories);
            string overrideFullPath = null;
            foreach (var filepath in filePathList)
            {
                if (Path.GetExtension(filepath) != ".cs")
                {
                    continue;
                }

                if (Path.GetFileName(filepath) != fileName)
                {
                    continue;
                }

                overrideFullPath = filepath;

                break;
            }

            if (overrideFullPath == null)
            {
                FileInfo info = new FileInfo(_path);
                string path = info.DirectoryName + "/" + fileName;
                FileUtil.SaveText(stringBuilder.ToString(), path);
                Debug.Log(path + "に生成しました");
            }
            else
            {
                FileUtil.SaveText(stringBuilder.ToString(), overrideFullPath);
                Debug.Log(overrideFullPath + "を上書きしました");
            }
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
                stringBuilder.Append("        public ").Append(pair.Value).Append(" ")
                    .Append(pair.Key.ToTitleUpperCase()).AppendLine(" { get; }");
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
                stringBuilder.Append("            ").Append(pair.Key.ToTitleUpperCase()).Append(" = ").Append(pair.Key)
                    .AppendLine(";");
            }

            stringBuilder.AppendLine("        }");

            stringBuilder.Append("        public ").Append(_modelName).Append("(").Append(_modelName)
                .AppendLine(" original)");
            stringBuilder.AppendLine("        {");
            foreach (var pair in _keyTypeMap)
            {
                stringBuilder.Append("            ").Append(pair.Key.ToTitleUpperCase()).Append(" = ")
                    .Append("original.").Append(pair.Key.ToTitleUpperCase()).AppendLine(";");
            }

            stringBuilder.AppendLine("        }");


            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("}");

            string projectPath = Directory.GetCurrentDirectory();
            string fileName = _modelName + ".cs";
            string[] filePathList = Directory.GetFiles(projectPath, "*.cs", System.IO.SearchOption.AllDirectories);
            string overrideFullPath = null;
            foreach (var filepath in filePathList)
            {
                if (Path.GetExtension(filepath) != ".cs")
                {
                    continue;
                }

                if (Path.GetFileName(filepath) != fileName)
                {
                    continue;
                }

                overrideFullPath = filepath;

                break;
            }

            if (overrideFullPath == null)
            {
                FileInfo info = new FileInfo(_path);
                string path = info.DirectoryName + "/" + fileName;
                FileUtil.SaveText(stringBuilder.ToString(), path);
                Debug.Log(path + "に生成しました");
            }
            else
            {
                FileUtil.SaveText(stringBuilder.ToString(), overrideFullPath);
                Debug.Log(overrideFullPath + "を上書きしました");
            }
        }

        void GeneratePrimaryKey()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("using Framework;");
            stringBuilder.AppendLine();
            stringBuilder.Append("namespace ").AppendLine(_namespace);
            stringBuilder.AppendLine("{");
            stringBuilder.Append("    public class ").Append(_playerDataName).Append("PrimaryKey : IPrimaryKey<")
                .Append(_primaryKeyName).Append(", ").Append(_modelName).AppendLine(">");
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
            stringBuilder.AppendLine("            return 1;");
            stringBuilder.AppendLine("        }");
            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("}");

            string projectPath = Directory.GetCurrentDirectory();
            string fileName = _primaryKeyName + ".cs";
            string[] filePathList = Directory.GetFiles(projectPath, "*.cs", System.IO.SearchOption.AllDirectories);
            string overrideFullPath = null;
            foreach (var filepath in filePathList)
            {
                if (Path.GetExtension(filepath) != ".cs")
                {
                    continue;
                }

                if (Path.GetFileName(filepath) != fileName)
                {
                    continue;
                }

                overrideFullPath = filepath;

                break;
            }

            if (overrideFullPath == null)
            {
                FileInfo info = new FileInfo(_path);
                string path = info.DirectoryName + "/" + fileName;
                FileUtil.SaveText(stringBuilder.ToString(), path);
                Debug.Log(path + "に生成しました");
            }
            else
            {
                FileUtil.SaveText(stringBuilder.ToString(), overrideFullPath);
                Debug.Log(overrideFullPath + "を上書きしました");
            }
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
            stringBuilder.Append("    public class ").Append(_datastoreName).Append(" : IPlayerDataStore<")
                .Append(_modelName).Append(", ").Append(_primaryKeyName).AppendLine(">");
            stringBuilder.AppendLine("    {");
            stringBuilder.Append("        const string FILE_PATH = \"").Append(_playerDataName).AppendLine(".json\";");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("        [Serializable]");
            stringBuilder.AppendLine("        class Entities");
            stringBuilder.AppendLine("        {");
            stringBuilder.Append("            public ").Append(_entityName).AppendLine("[] Values = null;");
            stringBuilder.AppendLine();
            stringBuilder.Append("            public static Entities ConvertFromMap(Dictionary<")
                .Append(_primaryKeyName).Append(", ").Append(_modelName).AppendLine("> map)");
            stringBuilder.AppendLine("            {");
            stringBuilder.AppendLine("                Entities entities = new Entities();");
            stringBuilder.Append("                entities.Values = new ").Append(_entityName)
                .AppendLine("[map.Count];");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("                int index = 0;");
            stringBuilder.AppendLine("                foreach (var model in map.Values)");
            stringBuilder.AppendLine("                {");
            stringBuilder.Append("                    entities.Values[index] = new ").Append(_entityName)
                .AppendLine("(model);");
            stringBuilder.AppendLine("                    index++;");
            stringBuilder.AppendLine("                }");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("                return entities;");
            stringBuilder.AppendLine("            }");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("            public static Entities ConvertFromJson(string json)");
            stringBuilder.AppendLine("            {");
            stringBuilder.AppendLine("                Entities entities =  JsonUtility.FromJson<Entities>(json);");
            stringBuilder.AppendLine("                return entities;");
            stringBuilder.AppendLine("            }");
            stringBuilder.AppendLine("        }");
            stringBuilder.AppendLine();
            stringBuilder.Append("        public void Save(").Append(_modelName).AppendLine(" model)");
            stringBuilder.AppendLine("        {");
            stringBuilder.Append("            Dictionary<").Append(_primaryKeyName).Append(", ").Append(_modelName)
                .AppendLine("> map = Load();");
            stringBuilder.Append("            ").Append(_primaryKeyName).Append(" primaryKey = new ")
                .Append(_primaryKeyName).AppendLine("();");
            stringBuilder.AppendLine("            primaryKey.Setup(model);");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("            if (map.ContainsKey(primaryKey))");
            stringBuilder.AppendLine("            {");
            stringBuilder.AppendLine("                map[primaryKey] = model;");
            stringBuilder.AppendLine("            }");
            stringBuilder.AppendLine("            else");
            stringBuilder.AppendLine("            {");
            stringBuilder.AppendLine("                map.Add(primaryKey, model);");
            stringBuilder.AppendLine("            }");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("            string json = JsonUtility.ToJson (Entities.ConvertFromMap(map));");
            stringBuilder.AppendLine(
                "            string filePath = $\"{Application.persistentDataPath}/{FILE_PATH}\";");
            stringBuilder.AppendLine("            StreamWriter writer = File.CreateText(filePath);");
            stringBuilder.AppendLine("            writer.Write(json);");
            stringBuilder.AppendLine("            writer.Close();");
            stringBuilder.AppendLine("            DebugLog.Normal(this.GetType() + \"を保存しました。\" + filePath);");
            stringBuilder.AppendLine("        }");
            stringBuilder.AppendLine();
            stringBuilder.Append("       public Dictionary<").Append(_primaryKeyName).Append(", ").Append(_modelName)
                .AppendLine("> Load()");
            stringBuilder.AppendLine("        {");
            stringBuilder.AppendLine(
                "            if (!File.Exists($\"{Application.persistentDataPath}/{FILE_PATH}\"))");
            stringBuilder.AppendLine("            {");
            stringBuilder.AppendLine("                DebugLog.Normal(FILE_PATH + \"が存在していません\");");
            stringBuilder.Append("                return new Dictionary<").Append(_primaryKeyName).Append(", ")
                .Append(_modelName).AppendLine(">();");
            stringBuilder.AppendLine("            }");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(
                "            string assetsPath = $\"{Application.persistentDataPath}/{FILE_PATH}\";");
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
            stringBuilder.Append("                return new Dictionary<").Append(_primaryKeyName).Append(", ")
                .Append(_modelName).AppendLine(">();");
            stringBuilder.AppendLine("            }");
            stringBuilder.AppendLine();
            stringBuilder.Append("            var map = new Dictionary<").Append(_primaryKeyName).Append(", ")
                .Append(_modelName).AppendLine(">();");
            stringBuilder.AppendLine("            var parse = JsonUtility.FromJson<Entities>(json);");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("            foreach (var entity in parse.Values)");
            stringBuilder.AppendLine("            {");
            stringBuilder.Append("                ").Append(_modelName).Append(" model = new ").Append(_modelName)
                .AppendLine("(");
            int i = 0;
            foreach (var pair in _keyTypeMap)
            {
                if (_keyTypeMap.Count - 1 <= i)
                {
                    stringBuilder.Append("                    entity.").Append(pair.Key.ToTitleUpperCase())
                        .AppendLine(");");
                }
                else
                {
                    stringBuilder.Append("                    entity.").Append(pair.Key.ToTitleUpperCase())
                        .AppendLine(",");
                }

                i++;
            }

            stringBuilder.AppendLine();
            stringBuilder.Append("                ").Append(_primaryKeyName).Append(" primaryKey = new ")
                .Append(_primaryKeyName).AppendLine("();");
            stringBuilder.AppendLine("                primaryKey.Setup(model);");
            stringBuilder.AppendLine("                map.Add(primaryKey, model);");
            stringBuilder.AppendLine("            }");
            stringBuilder.AppendLine("            return map;");
            stringBuilder.AppendLine("        }");
            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("}");

            string projectPath = Directory.GetCurrentDirectory();
            string fileName = _datastoreName + ".cs";
            string[] filePathList = Directory.GetFiles(projectPath, "*.cs", System.IO.SearchOption.AllDirectories);
            string overrideFullPath = null;
            foreach (var filepath in filePathList)
            {
                if (Path.GetExtension(filepath) != ".cs")
                {
                    continue;
                }

                if (Path.GetFileName(filepath) != fileName)
                {
                    continue;
                }

                overrideFullPath = filepath;

                break;
            }

            if (overrideFullPath == null)
            {
                FileInfo info = new FileInfo(_path);
                string path = info.DirectoryName + "/" + fileName;
                FileUtil.SaveText(stringBuilder.ToString(), path);
                Debug.Log(path + "に生成しました");
            }
            else
            {
                FileUtil.SaveText(stringBuilder.ToString(), overrideFullPath);
                Debug.Log(overrideFullPath + "を上書きしました");
            }
        }

        void GenerateRepository()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("using Framework;");
            stringBuilder.AppendLine();
            stringBuilder.Append("namespace ").AppendLine(_namespace);
            stringBuilder.AppendLine("{");
            stringBuilder.Append("    public class ").Append(_repositoryName).Append(" : PlayerBaseRepository<")
                .Append(_modelName).Append(", ").Append(_datastoreName).Append(", ").Append(_primaryKeyName)
                .AppendLine(">");
            stringBuilder.AppendLine("    {");
            stringBuilder.Append("        public static ").Append(_repositoryName).AppendLine(" I { get; }");
            stringBuilder.AppendLine();
            stringBuilder.Append("        static ").Append(_repositoryName).AppendLine("()");
            stringBuilder.AppendLine("        {");
            stringBuilder.Append("            I = new ").Append(_repositoryName).AppendLine("();");
            stringBuilder.AppendLine("        }");
            stringBuilder.AppendLine();
            stringBuilder.Append("        ").Append(_repositoryName).Append("() : base(new ").Append(_datastoreName)
                .AppendLine("()) {}");
            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("}");

            string projectPath = Directory.GetCurrentDirectory();
            string fileName = _repositoryName + ".cs";
            string[] filePathList = Directory.GetFiles(projectPath, "*.cs", System.IO.SearchOption.AllDirectories);
            string overrideFullPath = null;
            foreach (var filepath in filePathList)
            {
                if (Path.GetExtension(filepath) != ".cs")
                {
                    continue;
                }

                if (Path.GetFileName(filepath) != fileName)
                {
                    continue;
                }

                overrideFullPath = filepath;

                break;
            }

            if (overrideFullPath == null)
            {
                FileInfo info = new FileInfo(_path);
                string path = info.DirectoryName + "/" + fileName;
                FileUtil.SaveText(stringBuilder.ToString(), path);
                Debug.Log(path + "に生成しました");
            }
            else
            {
                FileUtil.SaveText(stringBuilder.ToString(), overrideFullPath);
                Debug.Log(overrideFullPath + "を上書きしました");
            }
        }
    }
}

#endif