#if UNITY_EDITOR
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    public class GenerateCleanArchitectureCodeEditorWindow : EditorWindow
    {
        const string ITEM_NAME = "Framework/CleanArchitecture Generate Window";
        const string WINDOW_TITLE = "CleanArchitecture Generate Window";
        
        string _namespace;
        string _className;
        bool _existView = true;

        [MenuItem(ITEM_NAME)]
        static void Open()
        {
            GetWindow<GenerateCleanArchitectureCodeEditorWindow>(true, WINDOW_TITLE);
        }

        void OnGUI()
        {
            _namespace = EditorGUILayout.TextField("namespace", _namespace);
            _className = EditorGUILayout.TextField("class name", _className);
            _existView = EditorGUILayout.Toggle("Viewが存在する", _existView);

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
            if (string.IsNullOrEmpty(_namespace))
            {
                Debug.Log("namespaceが設定されてません");
                return false;
            }
            
            if (string.IsNullOrEmpty(_className))
            {
                Debug.Log("クラス名が設定されてません");
                return false;
            }
            
            return true;
        }

        void Execute()
        {
            if (_existView)
            {
                GenerateUseCase();
                GenerateContainer();
                GeneratePresenter();
                GenerateView();
            }
            else
            {
                GenerateUseCaseNoUseView();
                GenerateContainerNoUseView();
            }
        }

        void GenerateUseCase()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("using Framework;");
            stringBuilder.AppendLine();
            stringBuilder.Append("namespace ").AppendLine(_namespace);
            stringBuilder.AppendLine("{");
            stringBuilder.Append("    public class ").Append(_className).Append("UseCase : UseCaseBase<").Append(_className).Append("Container, ").Append(_className).Append("Presenter, ").Append(_className).AppendLine("View>");
            stringBuilder.AppendLine("    {");
            stringBuilder.Append("        public ").Append(_className).Append("UseCase(").Append(_className).AppendLine("Container container):base(container)");
            stringBuilder.AppendLine("        {");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("        }");
            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("}");

            string path = Application.dataPath + "/Framework/Script/CleanArchitecture/Editor/" + _className + "UseCase.cs";
            FileUtil.SaveText(stringBuilder.ToString(), path);
            Debug.Log(path+"に生成しました");
        }

        void GenerateContainer()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("using Framework;");
            stringBuilder.AppendLine();
            stringBuilder.Append("namespace ").AppendLine(_namespace);
            stringBuilder.AppendLine("{");
            stringBuilder.Append("    public class ").Append(_className).Append("Container : IContainer<").Append(_className).Append("Presenter, ").Append(_className).AppendLine("View> ");
            stringBuilder.AppendLine("    {");
            stringBuilder.Append("        public ").Append(_className).AppendLine("Presenter Presenter { get; }");
            stringBuilder.Append("        public ").Append(_className).Append("Container(").Append(_className).AppendLine("Presenter presenter)");
            stringBuilder.AppendLine("        {");
            stringBuilder.AppendLine("            Presenter = presenter;");
            stringBuilder.AppendLine("        }");
            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("}");

            string path = Application.dataPath + "/Framework/Script/CleanArchitecture/Editor/" + _className + "Container.cs";
            FileUtil.SaveText(stringBuilder.ToString(), path);
            Debug.Log(path+"に生成しました");
        }
        
        void GeneratePresenter()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("using Framework;");
            stringBuilder.AppendLine("using UnityEngine;");
            stringBuilder.AppendLine();
            stringBuilder.Append("namespace ").AppendLine(_namespace);
            stringBuilder.AppendLine("{");
            stringBuilder.Append("    public class ").Append(_className).Append("Presenter : MonoBehaviour, IPresenter<").Append(_className).AppendLine("View>");
            stringBuilder.AppendLine("    {");
            stringBuilder.AppendLine("        [SerializeField]");
            stringBuilder.Append("        ").Append(_className).AppendLine("View _view = null;");
            stringBuilder.Append("        public ").Append(_className).AppendLine("View  View => _view;");
            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("}");

            string path = Application.dataPath + "/Framework/Script/CleanArchitecture/Editor/" + _className + "Presenter.cs";
            FileUtil.SaveText(stringBuilder.ToString(), path);
            Debug.Log(path+"に生成しました");
        }
        
        void GenerateView()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("using Framework;");
            stringBuilder.AppendLine("using UnityEngine;");
            stringBuilder.AppendLine("using UnityEngine.UI;");
            stringBuilder.AppendLine();
            stringBuilder.Append("namespace ").AppendLine(_namespace);
            stringBuilder.AppendLine("{");
            stringBuilder.Append("    public class ").Append(_className).AppendLine("View : MonoBehaviour, IView");
            stringBuilder.AppendLine("    {");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("}");

            string path = Application.dataPath + "/Framework/Script/CleanArchitecture/Editor/" + _className + "View.cs";
            FileUtil.SaveText(stringBuilder.ToString(), path);
            Debug.Log(path+"に生成しました");
        }

        void GenerateUseCaseNoUseView()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("using Framework;");
            stringBuilder.AppendLine();
            stringBuilder.Append("namespace ").AppendLine(_namespace);
            stringBuilder.AppendLine("{");
            stringBuilder.Append("    public class ").Append(_className).Append("UseCase : UseCaseBase<").Append(_className).AppendLine("Container>");
            stringBuilder.AppendLine("    {");
            stringBuilder.Append("        public ").Append(_className).Append("UseCase(").Append(_className).AppendLine("Container container):base(container)");
            stringBuilder.AppendLine("        {");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("        }");
            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("}");

            string path = Application.dataPath + "/Framework/Script/CleanArchitecture/Editor/" + _className + "UseCase.cs";
            FileUtil.SaveText(stringBuilder.ToString(), path);
            Debug.Log(path+"に生成しました");
        }
        
        void GenerateContainerNoUseView()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("using Framework;");
            stringBuilder.AppendLine();
            stringBuilder.Append("namespace ").AppendLine(_namespace);
            stringBuilder.AppendLine("{");
            stringBuilder.Append("    public class ").Append(_className).AppendLine("Container : IContainer");
            stringBuilder.AppendLine("    {");
            stringBuilder.Append("        public ").Append(_className).AppendLine("Container()");
            stringBuilder.AppendLine("        {");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("        }");
            stringBuilder.AppendLine("    }");
            stringBuilder.AppendLine("}");

            string path = Application.dataPath + "/Framework/Script/CleanArchitecture/Editor/" + _className + "Container.cs";
            FileUtil.SaveText(stringBuilder.ToString(), path);
            Debug.Log(path+"に生成しました");
        }
    }    
}

#endif