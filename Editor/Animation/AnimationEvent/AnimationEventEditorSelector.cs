#if UNITY_EDITOR
using Framework;
using UnityEditor;
using UnityEngine;

namespace FrameworkEditor
{
    public class AnimationEventEditorSelector
    {
        public AnimationEvents AnimationEvents { get; private set; }

        public bool NotSelectedAnimationEvents => AnimationEvents == null;

        AnimationClipEvents _animationClipEvents;

        public AnimationClipEvents AnimationClipEvents
        {
            get => _animationClipEvents;

            set
            {
                _animationClipEvents = value;
                Selection.activeObject = _animationClipEvents;
            }
        }

        AnimationEventObject _animationEventObject;
        
        public AnimationEventObject AnimationEventObject
        {
            get => _animationEventObject;

            set
            {
                _animationEventObject = value;
                Selection.activeObject = _animationEventObject;
            }
        }

        public AnimationEventInvoker AnimationEventInvoker { get; private set; }
        public AnimationEventEditorAnimator Animator { get; private set; }
        AnimationEventEditorWindowInfo EditorWindowInfo { get; set; }

        public bool DisablePlay()
        {
            if (AnimationClipEvents == null)
            {
                return true;
            }

            return !Animator.CanPlay();
        }

        public AnimationEventEditorSelector(AnimationEventEditorWindowInfo editorWindowInfo)
        {
            EditorWindowInfo = editorWindowInfo;
            Selection.selectionChanged = OnSelectionChanged;   
            Animator = new AnimationEventEditorAnimator();
        }

        void OnSelectionChanged()
        {
            OnSelectionChangedProject();
            OnSelectionChangedHierarchy();
            EditorWindowInfo.EditorWindow.Repaint();
        }

        void OnSelectionChangedProject()
        {
            if (Selection.activeObject is AnimationEvents)
            {
                AnimationEvents = Selection.activeObject as AnimationEvents;
            }
            else if (Selection.activeObject is AnimationClipEvents)
            {
                _animationClipEvents = Selection.activeObject as AnimationClipEvents;
            }
            else if (Selection.activeObject is AnimationEventObject)
            {
                _animationEventObject = Selection.activeObject as AnimationEventObject;
            }
        }

        void OnSelectionChangedHierarchy()
        {
            if (Selection.activeGameObject == null)
            {
                return;
            }

            var tempAnimationEventInvoker = Selection.activeGameObject.GetComponent<AnimationEventInvoker>();

            if (tempAnimationEventInvoker == null)
            {
                return;
            }

            foreach (var go in Object.FindObjectsOfType<AnimationEventInvoker>())
            {
                if (go.GetInstanceID() == tempAnimationEventInvoker.GetInstanceID())
                {
                    AnimationEventInvoker = tempAnimationEventInvoker;
                    AnimationEvents = AnimationEventInvoker.AnimationEvents;
                    Animator.Setup(AnimationEventInvoker);
                    break;
                }
            }
        }
        
    }
}

#endif