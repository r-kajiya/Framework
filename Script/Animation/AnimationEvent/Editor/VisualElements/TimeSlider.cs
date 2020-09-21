#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Framework
{
    public class TimeSlider : VisualElement
    {
        TimeArea _timeArea;

        public TimeSlider()
        {
            // VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Framework/Script/Animation/AnimationEvent/Editor/VisualElements/TimeSlider.uxml");
            // visualTree.CloneTree(this);
            //
            // StyleSheet stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Framework/Script/Animation/AnimationEvent/Editor/VisualElements/TimeSlider.uss");
            // styleSheets.Add(stylesheet);
            //
            // AddToClassList(TextElement.ussClassName);
            
            Add(new IMGUIContainer(() =>
            {
                // if(GUILayout.Button("IMGUIButton", GUILayout.Width( 1000 ),GUILayout.Height( 10)))
                // {
                //     Debug.Log("GUILayout.Buttonが押された");
                // }
                
                GUILayout.Box("IMGUIContainerBox", GUILayout.Width( 1000 ),GUILayout.Height( 1));
                
                // if (_timeArea == null)
                // {
                //     _timeArea = new TimeArea(false);
                // }
                //
                // _timeArea.TimeRuler(new Rect(new Vector2(0,0), new Vector2(1000, 100)), 12);

                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    GL.Begin(GL.QUADS);
                }
                else
                {
                    GL.Begin(GL.LINES);
                }
                
                for (int x = 0; x < 100; x++)
                {
                    if (Application.platform == RuntimePlatform.WindowsEditor)
                    {
                        GL.Color(Color.white);
                        GL.Vertex(new Vector3(x - 0.5f, 0, 0));
                        GL.Vertex(new Vector3(x + 0.5f, 0, 0));
                        GL.Vertex(new Vector3(x + 0.5f, 200, 0));
                        GL.Vertex(new Vector3(x - 0.5f, 200, 0));
                    }
                    else
                    {
                        GL.Color(Color.white);
                        GL.Vertex(new Vector3(x * 10, 0, 0));
                        GL.Vertex(new Vector3(x * 10, 200, 0));
                    }   
                }
                
                GL.End();
                
                for (int i = 0; i < 100; i++)
                {
                    int frame = Mathf.RoundToInt(i);
                    // Important to take floor of positions of GUI stuff to get pixel correct alignment of
                    // stuff drawn with both GUI and Handles/GL. Otherwise things are off by one pixel half the time.
                    
                    string label = i.ToString();
                    GUI.Label(new Rect(i * 10, -1, 40, 20), label, "AnimationTimelineTick");
                }
            }));

            // if (Application.platform == RuntimePlatform.WindowsEditor)
            // {
            //     GL.Begin(GL.QUADS);
            // }
            // else
            // {
            //     GL.Begin(GL.LINES);
            // }
            //
            // for (int x = 0; x < 100; x++)
            // {
            //     if (Application.platform == RuntimePlatform.WindowsEditor)
            //     {
            //         GL.Color(Color.white);
            //         GL.Vertex(new Vector3(x - 0.5f, 100, 0));
            //         GL.Vertex(new Vector3(x + 0.5f, 100, 0));
            //         GL.Vertex(new Vector3(x + 0.5f, 200, 0));
            //         GL.Vertex(new Vector3(x - 0.5f, 200, 0));
            //     }
            //     else
            //     {
            //         GL.Color(Color.white);
            //         GL.Vertex(new Vector3(x * 10, 100, 0));
            //         GL.Vertex(new Vector3(x * 10, 200, 0));
            //     }   
            // }
            //
            // GL.End();
        }
    }   
}

#endif
