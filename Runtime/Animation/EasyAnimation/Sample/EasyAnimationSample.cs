using System;
using UnityEngine;
using UnityEngine.UI;

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

        [SerializeField]
        Text _horizontalText;
        
        [SerializeField]
        Text _verticalText;

        [SerializeField, Range(0.01f, 1.0f)]
        float _blendValue;

        float _horizontal;
        float _vertical;
        
        void Start()
        {
            _verticalText.text = _vertical.ToString();
            _horizontalText.text = _horizontal.ToString();
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

        public void Blend()
        {
            _easyAnimation.Blend("BasicMotionsBlendTree", 0.3f, 0.3f);
        }

        public void VerticalUp()
        {
            _vertical += _blendValue;
            _verticalText.text = _vertical.ToString();
            _easyAnimation.SetBlendParameter(_horizontal, _vertical);
        }
        
        public void VerticalDown()
        {
            _vertical -= _blendValue;
            _verticalText.text = _vertical.ToString();
            _easyAnimation.SetBlendParameter(_horizontal, _vertical);
        }
        
        public void HorizontalUp()
        {
            _horizontal += _blendValue;
            _horizontalText.text = _horizontal.ToString();
            _easyAnimation.SetBlendParameter(_horizontal, _vertical);
        }
        
        public void HorizontalDown()
        {
            _horizontal -= _blendValue;
            _horizontalText.text = _horizontal.ToString();
            _easyAnimation.SetBlendParameter(_horizontal, _vertical);
        }

        public void OnTS1xButton()
        {
            Time.timeScale = 1f;
        }
        
        public void OnTS05xButton()
        {
            Time.timeScale = 0.1f;
        }
        
        public void OnTS01xButton()
        {
            Time.timeScale = 0.01f;
        }
    }
}


