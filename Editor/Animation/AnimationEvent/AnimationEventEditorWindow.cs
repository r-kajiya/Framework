#if UNITY_EDITOR
using System;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace FrameworkEditor
{
    public class AnimationEventEditorWindow : OdinEditorWindow
    {
        [MenuItem("Framework/AnimationEventEditor")]
        public static void ShowWindow() => GetWindow<AnimationEventEditorWindow>("AnimationEventEditor");

        AnimationEventEditorPlayControlGUI _playControlGUI;

        AnimationEventEditorPlayControlGUI PlayControlGUI
        {
            get
            {
                if (_playControlGUI == null)
                {
                    _playControlGUI = new AnimationEventEditorPlayControlGUI();
                }

                return _playControlGUI;
            }
        }
        
        AnimationEventEditorClipControlGUI _clipControlGUI;

        AnimationEventEditorClipControlGUI ClipControlGUI
        {
            get
            {
                if (_clipControlGUI == null)
                {
                    _clipControlGUI = new AnimationEventEditorClipControlGUI();
                }

                return _clipControlGUI;
            }
        }

        AnimationEventEditorTimeAreaGUI _timeAreaGUI;

        AnimationEventEditorTimeAreaGUI TimeAreaGUI
        {
            get
            {
                if (_timeAreaGUI == null)
                {
                    _timeAreaGUI = new AnimationEventEditorTimeAreaGUI();
                }

                return _timeAreaGUI;
            }
        }

        AnimationEventEditorWindowInfo _editorWindowInfo;

        AnimationEventEditorWindowInfo EditorWindowInfo
        {
            get
            {
                if (_editorWindowInfo == null)
                {
                    _editorWindowInfo = new AnimationEventEditorWindowInfo(this);
                }

                return _editorWindowInfo;
            }
        }
        
        public override bool UseScrollView => false;

        protected override void DrawEditor(int index)
        {
            EditorWindowInfo.Update(position);
            
            using (new EditorGUI.DisabledScope(EditorWindowInfo.Selector.NotSelectedAnimationEvents))
            {
                // All
                GUILayout.BeginHorizontal();
            
                {// Left
                
                    GUILayout.BeginVertical(GUILayout.Width(AnimationEventEditorWindowInfo.DEFAULT_LEFT_WIDTH));
                    {
                        GUILayout.BeginHorizontal("AnimPlayToolbar");
                        PlayControlGUI.OnGUI(EditorWindowInfo);
                        GUILayout.EndHorizontal();
                    }
                
                    {
                        ClipControlGUI.OnGUI(EditorWindowInfo);
                    }
                
                    GUILayout.EndVertical();
                }
            
                {// Right
                    GUILayout.BeginVertical(); 
                    TimeAreaGUI.OnGUI(EditorWindowInfo);
                    GUILayout.EndHorizontal();
                }
                
                GUILayout.EndHorizontal();
            }
        }

        void Update()
        {
            if (EditorWindowInfo.IsPlaying)
            {
                Repaint();
            }
        }
    }
}

#endif