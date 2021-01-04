using Framework;
using UnityEngine;

namespace FrameworkEditor
{
    public class AnimationEventEditorAnimator
    {
        
        EasyAnimation _easyAnimation;
        AnimationEventInvoker _animationEventInvoker;
        string _playAnimationStateName;

        public void Setup(AnimationEventInvoker animationEventInvoker)
        {
            _easyAnimation = animationEventInvoker.GetComponent<EasyAnimation>();
            _easyAnimation.Initialize();
            _animationEventInvoker = animationEventInvoker;
        }

        public bool CanPlay()
        {
            if (_animationEventInvoker == null
                || _animationEventInvoker.AnimationEvents == null
                || _animationEventInvoker.AnimationEvents.AnimationClipEventsList.Count == 0)
            {
                return false;
            }

            // 一つでもアニメーションクリップを持っていたら
            // もっと言えば再生可能な状態であれば
            foreach (var animationClipEvents in _animationEventInvoker.AnimationEvents.AnimationClipEventsList)
            {
                if (animationClipEvents.AnimationClip != null)
                {
                    return true;
                }
            }

            return false;
        }

        public void Play(string stateName)
        {
            if (_easyAnimation == null)
            {
                return;
            }

            _easyAnimation.SetSpeed(1.0f);
            _easyAnimation.Play(stateName, 0);
            var state = _easyAnimation.GetState(stateName);
            
            if (!state.Clip.isLooping)
            {
                _playAnimationStateName = stateName;
            }
            else
            {
                _playAnimationStateName = null;
            }
        }

        public void Stop()
        {
            if (_easyAnimation == null)
            {
                return;
            }

            _easyAnimation.Stop();
            _playAnimationStateName = null;
        }

        public float GetTime()
        {
            if (_easyAnimation == null)
            {
                return 0.0f;
            }

            var currentState = _easyAnimation.GetCurrentState();

            if (currentState == null)
            {
                return 0.0f;
            }

            return currentState.GetTime();
        }

        public void Update()
        {
            if (_easyAnimation == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(_playAnimationStateName))
            {
                var currentState = _easyAnimation.GetCurrentState();
                if (currentState == null)
                {
                    Play(_playAnimationStateName);
                }
            }
            
            _easyAnimation.Animator.Update(Time.deltaTime);
        }

        public void ImportEvent()
        {
            _easyAnimation.ImportAnimationEvents(_animationEventInvoker.AnimationEvents);
        }

        public void ClearEvents()
        {
            
        }
        
    }
}