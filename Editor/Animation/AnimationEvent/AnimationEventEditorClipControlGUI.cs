#if UNITY_EDITOR
using System.Linq;
using Framework;
using UnityEditor;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace FrameworkEditor
{
    public class AnimationEventEditorClipControlGUI : AnimationEventEditorGUI
    {
        public override void OnGUI(AnimationEventEditorWindowInfo editorWindowInfo)
        {
            base.OnGUI(editorWindowInfo);

            if (EditorWindowInfo.Selector.NotSelectedAnimationEvents)
            {
                return;
            }

            DrawAnimationClips();
            DrawAddAnimationClip();
        }

        void DrawAnimationClips()
        {
            int animationClipCount = EditorWindowInfo.Selector.AnimationEvents.Count();
            for (int i = 0; i < animationClipCount; i++)
            {
                DrawAnimationClip(i, EditorWindowInfo.Selector.AnimationEvents.AnimationClipEventsList[i]);
            }
        }

        void DrawAnimationClip(int index, AnimationClipEvents animationClipEvents)
        {
            GUILayout.BeginHorizontal();
            {
                DrawRemoveAnimationClipButton(index);

                DrawAnimationClipControlButton(index, animationClipEvents);
            }
            GUILayout.EndHorizontal();
        }

        void DrawAddAnimationClip()
        {
            if (GUILayout.Button("Add"))
            {
                Undo.RecordObject(EditorWindowInfo.Selector.AnimationEvents, "Add AnimationClipEvent");
                EditorWindowInfo.Selector.AnimationEvents.AddDefault();
                EditorUtility.SetDirty(EditorWindowInfo.Selector.AnimationEvents);
                AssetDatabase.SaveAssets();
            }
        }
        
        void DrawRemoveAnimationClipButton(int index)
        {
            const float deleteButtonWidth = 20;
            Color defaultColor = GUI.color;
            GUI.color = Color.gray;
                
            if (GUILayout.Button("-", GUILayout.Width(deleteButtonWidth)))
            {
                Undo.RecordObject(EditorWindowInfo.Selector.AnimationEvents, "Remove AnimationClipEvent");
                EditorWindowInfo.Selector.AnimationEvents.RemoveAt(index);
                EditorWindowInfo.Selector.AnimationClipEvents = null;
                EditorWindowInfo.StopAnimation();
            }

            GUI.color = defaultColor;
        }

        void DrawAnimationClipControlButton(int index, AnimationClipEvents animationClipEvents)
        {
            string buttonName = "Don't have AnimationClip";
            Color defaultColor = GUI.color;

            if (animationClipEvents.AnimationClip == null)
            {
                GUI.color = Color.red;
            }
            else if (EditorWindowInfo.Selector.AnimationClipEvents == animationClipEvents)
            {
                buttonName = animationClipEvents.AnimationClip.name;
                GUI.color = Color.green;
            }
            else
            {
                buttonName = animationClipEvents.AnimationClip.name;
                GUI.color = defaultColor;
            }

            if (GUILayout.Button(buttonName))
            {
                if (EditorWindowInfo.IsPlaying)
                {
                    if (animationClipEvents.AnimationClip != null)
                    {
                        EditorWindowInfo.StopAnimation();
                    }
                }
                else
                {
                    EditorWindowInfo.StopAnimation();
                }
                
                EditorWindowInfo.Selector.AnimationClipEvents = animationClipEvents;
                
            }
                
            GUI.color = defaultColor;
        }
    }
}

#endif