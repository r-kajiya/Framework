#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Framework
{
    public class AnimationEventEditorWindow : EditorWindow
    {
        [MenuItem("Framework/AnimationEventEditor")]
        public static void ShowWindow() => GetWindow<AnimationEventEditorWindow>("AnimationEventEditorWindow");

        VisualTreeAsset _virtualTree;

        VisualTreeAsset VisualTree
        {
            get
            {
                if (_virtualTree == null)
                {
                    _virtualTree = Resources.Load<VisualTreeAsset>("UXMLAnimationEventEditorWindow");
                }
                return _virtualTree;
            }
        }

        StyleSheet _styleSheet;
        
        StyleSheet StyleSheet
        {
            get
            {
                if (_styleSheet == null)
                {
                    _styleSheet = Resources.Load<StyleSheet>("CSSAnimationEventEditorWindow");
                }
                return _styleSheet;
            }
        }

        AnimationEventTimeline _timeline;
        
        AnimationEventTimeline Timeline
        {
            get
            {
                if (_timeline == null)
                {
                    _timeline = new AnimationEventTimeline();
                }
                return _timeline;
            }
        }

        void OnEnable()
        {
            Setup();
        }

        void Setup()
        {
            rootVisualElement.styleSheets.Add(StyleSheet);
            VisualTree.CloneTree(rootVisualElement);
            rootVisualElement.Bind(new SerializedObject(this));

            rootVisualElement.Query<Button>()
                .ForEach(x => {
                    if(x.name == "scene_button"){
                        x.clickable.clicked += () => Debug.Log(x.name);
                    }
                    
                    if(x.name == "play_button"){
                        x.clickable.clicked += () => Debug.Log(x.name);
                    }
                    
                    if(x.name == "stop_button"){
                        x.clickable.clicked += () => Debug.Log(x.name);
                    }
                });
            
            rootVisualElement.Query<TextField>()
                .ForEach(x => {
                    Debug.Log(x.name);
                });
            
            rootVisualElement.Query<SliderInt>()
                .ForEach(x => {
                    Debug.Log(x.name);
                });
            
            var animationClipField = rootVisualElement.Q<ObjectField>("animation_clip_field");
            animationClipField.value = null;
            animationClipField.objectType = typeof(AnimationClip);
            
            animationClipField.RegisterCallback<ChangeEvent<AnimationClip>>(changeEvent =>
            {
                
            });
            
            var frameRateType = rootVisualElement.Q<EnumField>("frame_rate_type");
            frameRateType.Init(AnimationEventFrameRate.F30);

            frameRateType.RegisterCallback<ChangeEvent<Enum>>((evt) =>
            {
                
            });
            
            var animationFrameSlider = rootVisualElement.Q<SliderInt>("frame_slider");
            animationFrameSlider.value = 1;
            
            animationFrameSlider.RegisterCallback<ChangeEvent<int>>((evt) =>
            {
                
            });

            // CreateTimeline();
            CreateFrameButtons(100);
            // rootVisualElement.Add(new IMGUIContainer(() =>
            // {
            //     if(GUILayout.Button("IMGUIButton"))
            //     {
            //         Debug.Log("GUILayout.Buttonが押された");
            //     }
            // }));
        }

        void CreateFrameButtons(int max)
        {
            var rightBoxSecond = rootVisualElement.Q<Box>("right_box_second");

            for (int i =0;i < max; i++)
            {
                Action action = () =>
                {
                };
            
                var csharpButton = new Button(action) { text = "" };
                rightBoxSecond.Add(csharpButton);
            }
        }

        void CreateTimeline()
        {
            var rightBoxSecond = rootVisualElement.Q<Box>("right_box_second");
            
            TimeSlider timeSlider = new TimeSlider();

            rightBoxSecond.Add(timeSlider);
        }
    }   
}

#endif