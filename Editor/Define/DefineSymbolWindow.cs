#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace FrameworkEditor
{
    public class DefineSymbolWindow : EditorWindow
    {
        class SymbolData
        {
            public string Name { get; private set; }
            public string Comment { get; private set; }
            public bool IsEnable { get; set; }
            
            public SymbolData(XmlNode node)
            {
                Name = node.Attributes["name"].Value;
                Comment = node.Attributes["comment"].Value;
            }
        }

        const string ITEM_NAME = "Framework/DefineSymbolの設定";
        const string WINDOW_TITLE = "Define Symbol";
        const string XML_PATH = "Assets/Framework/Script/Define/Editor/symbols.xml";
        static Vector2 _scrollPos;
        static SymbolData[] _symbolList;
        
        [MenuItem(ITEM_NAME)]
        static void Open()
        {
            var window = GetWindow<DefineSymbolWindow>(true, WINDOW_TITLE);
            window.Init();
        }

        void Init()
        {
            var document = new XmlDocument();
            document.Load(XML_PATH);

            var root = document.GetElementsByTagName("root")[0];
            var symbolList = new List<XmlNode>();

            foreach (XmlNode n in root.ChildNodes)
            {
                if (n.Name == "symbol")
                {
                    symbolList.Add(n);
                }
            }

            _symbolList = symbolList
                .Select(c => new SymbolData(c))
                .ToArray();

            var defineSymbols = PlayerSettings
                .GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup)
                .Split(';');

            foreach (var n in _symbolList)
            {
                n.IsEnable = defineSymbols.Any(c => c == n.Name);
            }
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            _scrollPos = EditorGUILayout.BeginScrollView(
                _scrollPos,
                GUILayout.Height(position.height)
            );
            foreach (var n in _symbolList)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                n.IsEnable = EditorGUILayout.Toggle(n.IsEnable, GUILayout.Width(16));
                if (GUILayout.Button("Copy"))
                {
                    EditorGUIUtility.systemCopyBuffer = n.Name;
                }

                EditorGUILayout.LabelField(n.Name, GUILayout.ExpandWidth(true), GUILayout.MinWidth(0));
                EditorGUILayout.LabelField(n.Comment, GUILayout.ExpandWidth(true), GUILayout.MinWidth(0));
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Save"))
            {
                var defineSymbols = _symbolList
                    .Where(c => c.IsEnable)
                    .Select(c => c.Name)
                    .ToArray();

                PlayerSettings.SetScriptingDefineSymbolsForGroup(
                    EditorUserBuildSettings.selectedBuildTargetGroup,
                    string.Join(";", defineSymbols)
                );
                Close();
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }
}

#endif