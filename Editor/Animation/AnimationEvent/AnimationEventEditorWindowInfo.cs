#if UNITY_EDITOR
using Framework;
using UnityEditor;
using UnityEngine;

namespace FrameworkEditor
{
    public class AnimationEventEditorWindowInfo
    {
        public const float DEFAULT_LEFT_WIDTH = 300.0f;
        
        Rect _position;
        public Rect Position => _position;
        public float DefaultRightWidth => _position.width - DEFAULT_LEFT_WIDTH;
        public bool IsPlaying { get; set; }
        AnimationEventEditorSelector _selector;
        public AnimationEventEditorSelector Selector
        {
            get
            {
                if (_selector == null)
                {
                    _selector = new AnimationEventEditorSelector(this);
                }

                return _selector;
            }
        }

        public int TickCount
        {
            get
            {
                if (Selector.AnimationEvents == null ||
                    Selector.AnimationClipEvents == null || 
                    Selector.AnimationClipEvents.AnimationClip == null)
                {
                    return 0;
                }
                
                return (int)(Selector.AnimationClipEvents.AnimationClip.length * 100.0f);
            }
        }
        
        public AnimationEventEditorWindow EditorWindow { get; set; }

        public AnimationEventEditorWindowInfo(AnimationEventEditorWindow animationEventEditorWindow)
        {
            EditorWindow = animationEventEditorWindow;
        }

        public void Update(Rect position)
        {
            _position = position;
            Selector.Animator.Update();
        }

        public void StopAnimation()
        {
            IsPlaying = false;
            Selector.Animator.Stop();
        }
    }
}

#endif