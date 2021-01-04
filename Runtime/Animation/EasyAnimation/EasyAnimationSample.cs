using UnityEngine;

namespace Framework
{
    public class EasyAnimationSample : MonoBehaviour
    {
        [SerializeField]
        EasyAnimation _easyAnimation = null;

        [SerializeField]
        AnimationClip _addClip = null;

        [SerializeField, Range(0, 10)]
        float time = 0;
        
        [SerializeField, Range(0, 3)]
        float normalizedTransitionDuration = 0.3f;
        
        void Start()
        {
            _easyAnimation.Initialize();
        }

        void OnDestroy()
        {
            _easyAnimation.Release();
        }

        public void Idle()
        {
            _easyAnimation.Play("BasicMotions@Idle01", time);
        }
        
        public void Jump()
        {
            _easyAnimation.Play("BasicMotions@Jump01 [RootMotion]", time);
        }
        
        public void Run()
        {
            _easyAnimation.Play("BasicMotions@Run01", time);
        }
        
        public void IdleCrossFade()
        {
            _easyAnimation.CrossFade(0, time, normalizedTransitionDuration);
        }
        
        public void JumpCrossFade()
        {
            _easyAnimation.CrossFade(1, time, normalizedTransitionDuration);
        }
        
        public void RunCrossFade()
        {
            _easyAnimation.CrossFade(2, time, normalizedTransitionDuration);
        }

        public void AddClip()
        {
            _easyAnimation.Add(_addClip, "add");
        }
        
        public void RemoveClip()
        {
            _easyAnimation.Remove("add");
        }

        public void PlayAdd()
        {
            _easyAnimation.Play("add", 0);
        }

        public void SetSpeed05()
        {
            _easyAnimation.SetSpeed(0.5f);
        }
        
        public void SetSpeed10()
        {
            _easyAnimation.SetSpeed(1.0f);
        }
        
        public void SetSpeed20()
        {
            _easyAnimation.SetSpeed(2.0f);
        }

        public void Pause()
        {
            _easyAnimation.Pause();
        }

        public void Resume()
        {
            _easyAnimation.Resume();
        }
    }
}


