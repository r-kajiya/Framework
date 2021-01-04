using System;
using UnityEngine;

namespace Framework
{
    public class AnimationEventInvoker : MonoBehaviour
    {
        [SerializeField]
        AnimationEvents _animationEvents;
        
        [SerializeField]
        bool _convertAutomaticallyOnAwake = false;

        public AnimationEvents AnimationEvents => _animationEvents;

        void Awake()
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

        public void PlayEffect(UnityEngine.Object obj)
        {
            // エフェクトキー,再生する場所(GameObject名),オフセット,回転値オフセット,スケールオフセット
            Debug.Log("PlayEffect");
        }

        public void PlaySound(UnityEngine.Object obj)
        {
            AnimationEvents.AnimationClipEventsList[0].ToAnimationEvents();
            var animationEventObject = obj as AnimationEventObject;
            Debug.Log("PlaySound");
            // 再生する音キー
        }
    }
}