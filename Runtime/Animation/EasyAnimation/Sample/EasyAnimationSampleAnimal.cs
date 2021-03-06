using System;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    public class EasyAnimationSampleAnimal : MonoBehaviour
    {
        [SerializeField]
        EasyAnimation _easyAnimation = null;

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

        [SerializeField]
        float transitionTime = 0.3f;

        public void Idle01CrossFade()
        {
            _easyAnimation.Play("Idle_01", 0f, transitionTime);
        }
        
        public void Idle02CrossFade()
        {
            _easyAnimation.Play("Idle_02", 0f, transitionTime);
        }
        
        public void Idle03CrossFade()
        {
            _easyAnimation.Play("Idle_03", 0f, transitionTime);
        }
        
        public void Idle04CrossFade()
        {
            _easyAnimation.Play("Idle_04", 0f, transitionTime);
        }
        
        static readonly string blendName = "LocomotionTest";
        static readonly string blendName2 = "LocomotionTest_2";
        
        public void Blend()
        {
            _easyAnimation.Blend(blendName, transitionTime);
        }
        
        public void Blend2()
        {
            _easyAnimation.Blend(blendName2, transitionTime);
        }

        public void VerticalUp()
        {
            _vertical += _blendValue;
            _verticalText.text = _vertical.ToString();
            _easyAnimation.SetBlendParameter(blendName, _horizontal, _vertical);
            _easyAnimation.SetBlendParameter(blendName2, _horizontal, _vertical);
        }
        
        public void VerticalDown()
        {
            _vertical -= _blendValue;
            _verticalText.text = _vertical.ToString();
            _easyAnimation.SetBlendParameter(blendName, _horizontal, _vertical);
            _easyAnimation.SetBlendParameter(blendName2, _horizontal, _vertical);
        }
        
        public void HorizontalUp()
        {
            _horizontal += _blendValue;
            _horizontalText.text = _horizontal.ToString();
            _easyAnimation.SetBlendParameter(blendName, _horizontal, _vertical);
            _easyAnimation.SetBlendParameter(blendName2, _horizontal, _vertical);
        }
        
        public void HorizontalDown()
        {
            _horizontal -= _blendValue;
            _horizontalText.text = _horizontal.ToString();
            _easyAnimation.SetBlendParameter(blendName, _horizontal, _vertical);
            _easyAnimation.SetBlendParameter(blendName2, _horizontal, _vertical);
        }
    }
}


