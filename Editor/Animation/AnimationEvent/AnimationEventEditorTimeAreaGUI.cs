#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Framework;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace FrameworkEditor
{
    public class AnimationEventEditorTimeAreaGUI : AnimationEventEditorGUI
    {
        float _zoomScale = 1.0f;
        Vector2 _scrollPos;
        AnimationEventEditorWindowInfo _editorWindowInfo;
        int _overlayLineXTick;

        public override void OnGUI(AnimationEventEditorWindowInfo editorWindowInfo)
        {
            base.OnGUI(editorWindowInfo);

            _editorWindowInfo = editorWindowInfo;
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            
            Rect timeAreaRect = GUILayoutUtility.GetRect(0, 0);

            DrawTimeAreaBackground(timeAreaRect);
            DrawTimeArea(timeAreaRect, _editorWindowInfo.Position.height);
            
            EditorGUILayout.EndScrollView();
            
            RemoveAnimationEventObject();
        }

        void RemoveAnimationEventObject()
        {
            if (EditorWindowInfo.Selector.AnimationEventObject == null
            || EditorWindowInfo.Selector.AnimationClipEvents == null)
            {
                return;
            }

            if (Event.current.keyCode == KeyCode.Backspace)
            {
                var animationEventObject = EditorWindowInfo.Selector.AnimationClipEvents.GetValue(EditorWindowInfo.Selector.AnimationEventObject.Frame);
                EditorWindowInfo.Selector.AnimationClipEvents.RemoveAt(EditorWindowInfo.Selector.AnimationEventObject.Frame);
                Object.DestroyImmediate(animationEventObject, true);
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(EditorWindowInfo.Selector.AnimationEvents));
                EditorWindowInfo.Selector.AnimationEventObject = null;
                Event.current.Use();
                EditorWindowInfo.Selector.Animator.ImportEvent();
            }
        }

        void DrawTimeAreaBackground(Rect rect)
        {
            GUIStyle timeRulerBackground = (GUIStyle) "TimeRulerBackground";
            GUI.Box(rect, GUIContent.none, timeRulerBackground);
        }

        void DrawTimeArea(Rect rect, float timeAreaHeight)
        {
            const float minTime = 0.01f;
            const float zoomScrollingPower = 0.02f;
            const float defaultSpace = 15.0f;
            const float margin = 20;
            const float timelineHeight = 20;
            const float lineHeight = 10;
            const int defaultSeparationSec = 5;
            const int minZoomSeparationSec = 20;
            const float separationMinZoomScale = 0.3f;
            
            _zoomScale = Zoom(_zoomScale, zoomScrollingPower);
            float space = defaultSpace * _zoomScale;
            int tickCount = EditorWindowInfo.TickCount;
            float timeAreaWidth = tickCount * space;
            int separationSec = defaultSeparationSec;
            if (_zoomScale < separationMinZoomScale)
            {
                separationSec = minZoomSeparationSec;
            }

            SirenixEditorGUI.DrawSolidRect(margin * 2 + timeAreaWidth, timelineHeight, Color.gray);
            
            for (int frame = 0; frame < tickCount; frame++)
            {
                bool separation = frame % separationSec != 0;
                float x = margin + space * frame + rect.xMin;
                float y = timelineHeight * 0.5f + lineHeight * 0.5f;

                DrawTimeline(x, y, lineHeight, separation);

                DrawButton(x, y, space, timelineHeight, 0, frame);
                
                if (separation)
                {
                    continue;
                }

                DrawTimelineSecText(x, y, space, lineHeight, minTime, frame);
            }

            if (EditorWindowInfo.IsPlaying)
            {
                UpdateTick();
            }
            
            DrawOverlayLine(rect,margin, space, timeAreaHeight);
        }

        void DrawTimeline(float x, float y, float lineHeight, bool separation)
        {
            float alpha = 1.0f;
            
            if (separation)
            {
                lineHeight *= 0.8f;
                alpha *= 0.5f;
            }
            
            SirenixEditorGUI.DrawVerticalLineSeperator(x, y, lineHeight, alpha);
        }

        void DrawButton(float x, float y, float space, float height, int layer, int frame)
        {
            const float buttonHeight = 30;
            
            float buttonX = x - space * 0.5f;
            float buttonY = y + height * 0.5f + layer * buttonHeight;
            float buttonWidth = space;
            Rect buttonPosition = new Rect(buttonX, buttonY, buttonWidth, buttonHeight);

            Color defaultColor = GUI.color;
            var animationEventObject = EditorWindowInfo.Selector.AnimationClipEvents.GetValue(frame);

            if (animationEventObject != null)
            {
                GUI.color = Color.red;
            }

            if (GUI.Button(buttonPosition, string.Empty))
            {
                if (animationEventObject == null)
                {
                    EditorWindowInfo.Selector.AnimationEventObject = EditorWindowInfo.Selector.AnimationClipEvents.AddAt(frame);
                }
                else
                {
                    EditorWindowInfo.Selector.AnimationEventObject = EditorWindowInfo.Selector.AnimationClipEvents.GetValue(frame);
                }
                
                EditorWindowInfo.Selector.Animator.ImportEvent();

                EditorUtility.SetDirty(EditorWindowInfo.Selector.AnimationEvents);
                AssetDatabase.SaveAssets();
            }

            GUI.color = defaultColor;
        }

        void DrawTimelineSecText(float x, float y, float space, float lineHeight, float minTime, int lineIndex)
        {
            const float textWidth = 40;
            const float textHeight = 10;
            const int fontSize = 10;
            
            float textX = x - space * 0.25f;
            float textY = y - lineHeight;
            
            string text = $"{minTime * lineIndex:F}";
            Rect textPosition = new Rect(textX, textY, textWidth, textHeight);
            GUIStyle textStyle = new GUIStyle();
            textStyle.fontSize = fontSize;
            GUI.Label(textPosition, text, textStyle);
        }

        void UpdateTick()
        {
            _overlayLineXTick = (int)(EditorWindowInfo.Selector.Animator.GetTime() * 100.0f);
        }

        void DrawOverlayLine(Rect rect, float margin, float space, float windowHeight)
        {
            if (rect == new Rect(new Vector2(), new Vector2(1.0f, 1.0f)))
            {
                return;
            }
            
            float overlayStartPosX = margin + rect.xMin;
            float overlayLineX = _overlayLineXTick * space + overlayStartPosX;
            
            const float overlayLineY = 0;
            SirenixEditorGUI.DrawVerticalLineSeperator(overlayLineX, overlayLineY, windowHeight, 1.0f);
        }
        
        float Zoom(float scale, float power)
        {
            const float scaleMin = 0.1f;
            const float scaleMax = 2.5f;
            
            var e = Event.current;
            if (e.type != EventType.ScrollWheel)
            {
                return scale;
            }
            
            float delta = Event.current.delta.x + Event.current.delta.y;
            delta = -delta;
            scale += delta * power;
            scale = Mathf.Min(scale, scaleMax);
            scale = Mathf.Max(scale, scaleMin);

            e.Use();

            return scale;
        }
    }
}

#endif