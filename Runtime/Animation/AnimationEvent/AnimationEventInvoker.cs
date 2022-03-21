using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{
    [ExecuteInEditMode]
    public class AnimationEventInvoker : MonoBehaviour
    {
        [SerializeField]
        AnimationEvents _animationEvents;
        
        [SerializeField]
        bool _convertAutomaticallyOnAwake = false;

        public AnimationEvents AnimationEvents => _animationEvents;

        void Start()
        {
            if (!_convertAutomaticallyOnAwake)
            {
                return;
            }
            
            var easyAnimation = GetComponent<EasyAnimation>();
            if (easyAnimation != null)
            {
                easyAnimation.ImportAnimationEvents(_animationEvents);
            }
        }

        // Sample
        public void AnimationEventObjectExecutorPlayEffect(Object obj)
        {
            var eventObj = obj as AnimationEventObject;

            if (eventObj == null)
            {
                Debug.LogError("AnimationEventInvoker:AnimationEventObjectExecutorPlayEffect:AnimationEventに設定されているオブジェクトがAnimationEventObjectExecutorPlayEffectではありません。" + name + ":" + _animationEvents.name);
                return;
            }

            var executor = eventObj.Executor as AnimationEventObjectExecutorPlayEffect;
            if (executor == null)
            {
                Debug.LogError("AnimationEventInvoker:AnimationEventObjectExecutorPlayEffect:AnimationEventに設定されているオブジェクトがAnimationEventObjectExecutorPlayEffectではありません。" + name + ":" + _animationEvents.name);
                return;
            }
            
            executor.Play(this);
        }
    }
}