using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    [RequireComponent(typeof(Animation))]
    public class EasyAnimation : MonoBehaviour
    {
        [SerializeField]
        Animation _animation = null;

        public Action onPlay;
        public Action onStop;
        public Action onFinish;

        bool isStop = true;

        public bool IsPlaying()
        {
            if (_animation.clip == null)
            {
                DebugLog.Error("アニメーションクリップがnullです" + "," + gameObject.name);
                return false;
            }

            return _animation.isPlaying;
        }

        public bool Play(string clipName = null)
        {
            if (_animation.clip == null)
            {
                DebugLog.Error("アニメーションクリップがnullです" + "," + gameObject.name);
                return false;
            }

            if (_animation.GetClip(clipName) == null)
            {
                DebugLog.Error("アニメーションクリップが存在しません。" + clipName + "," + gameObject.name);
                return false;
            }

            if (_animation.isPlaying)
            {
                DebugLog.Error("アニメーションは再生中です。" + _animation.clip.name + "," + gameObject.name);
                return false;
            }

            _animation.Play(clipName);
            onPlay?.Invoke();

            onPlay = null;

            isStop = false;

            return true;
        }

        public bool Stop()
        {
            if (_animation.clip == null)
            {
                DebugLog.Error("アニメーションクリップがnullです" + "," + gameObject.name);
                return false;
            }

            _animation.Stop();
            onStop?.Invoke();

            onPlay = null;
            onFinish = null;
            onStop = null;
            
            isStop = true;

            return true;
        }

        void LateUpdate()
        {
            if (isStop)
            {
                return;
            }

            if (_animation.isPlaying)
            {
                return;
            }

            onFinish?.Invoke();
            onStop?.Invoke();
            onFinish = null;
            onStop = null;
            isStop = true;
        }

#if UNITY_EDITOR
        public Animation Editor_Animation { get { return _animation; } set { _animation = value; } }
#endif
    }
}