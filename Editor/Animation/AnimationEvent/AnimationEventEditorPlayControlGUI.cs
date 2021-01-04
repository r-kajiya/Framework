#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Framework;

namespace FrameworkEditor
{
    public class AnimationEventEditorPlayControlGUI: AnimationEventEditorGUI
    {
        const float BUTTON_WIDTH = 25.0f;
        
        public override void OnGUI(AnimationEventEditorWindowInfo editorWindowInfo)
        {
            base.OnGUI(editorWindowInfo);
            
            GUIContent firstKeyContent = EditorGUIUtility.TrIconContent("Animation.FirstKey", "Go to the beginning of the animation clip.");

            if (GUILayout.Button(firstKeyContent, EditorStyles.toolbarButton, GUILayout.Width(BUTTON_WIDTH)))
            {
                // 初回のキーに戻る
            }
            
            GUIContent prevKeyContent = EditorGUIUtility.TrIconContent("Animation.PrevKey", "Go to previous keyframe.");

            if (GUILayout.Button(prevKeyContent, EditorStyles.toolbarButton, GUILayout.Width(BUTTON_WIDTH)))
            {
                // 一つ前のキーに戻る
            }

            using (new EditorGUI.DisabledScope(EditorWindowInfo.Selector.DisablePlay()))
            {
                // 再生ボタン
                PlayButtonOnGUI();
            }
            
            GUIContent nextKeyContent = EditorGUIUtility.TrIconContent("Animation.NextKey", "Go to next keyframe.");
            
            if (GUILayout.Button(nextKeyContent, EditorStyles.toolbarButton, GUILayout.Width(BUTTON_WIDTH)))
            {
                // 一つ次のキーへ
            }
            
            GUIContent lastKeyContent = EditorGUIUtility.TrIconContent("Animation.LastKey", "Go to the end of the animation clip.");

            if (GUILayout.Button(lastKeyContent, EditorStyles.toolbarButton, GUILayout.Width(BUTTON_WIDTH)))
            {
                // 最後のキーへ
            }

            const float textWidth = 175.0f;
            
            if (EditorWindowInfo.Selector.NotSelectedAnimationEvents)
            {
                EditorWindowInfo.IsPlaying = false;
                GUILayout.Label("not selected", EditorStyles.miniLabel, GUILayout.MaxWidth(textWidth));
            }
            else
            {
                GUILayout.Label(EditorWindowInfo.Selector.AnimationEvents.name, EditorStyles.miniLabel, GUILayout.MaxWidth(textWidth));   
            }
        }
        
        void PlayButtonOnGUI()
        {
            GUIContent playContent = EditorGUIUtility.TrIconContent("Animation.Play", "Play the animation clip.");
            EditorGUI.BeginChangeCheck();
            EditorWindowInfo.IsPlaying = GUILayout.Toggle(EditorWindowInfo.IsPlaying, playContent, EditorStyles.toolbarButton, GUILayout.Width(BUTTON_WIDTH));
            
            if (EditorGUI.EndChangeCheck())
            {
                if (EditorWindowInfo.IsPlaying)
                {
                    EditorWindowInfo.Selector.Animator.Play(EditorWindowInfo.Selector.AnimationClipEvents.AnimationName);
                }
                else
                {
                    EditorWindowInfo.Selector.Animator.Stop();
                }
            }
        }
    }
}

#endif