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

        public void IdleCrossFade()
        {
            _easyAnimation.CrossFade(0);
        }
        
        public void Blend()
        {
            _easyAnimation.Blend("Locomotion", 0.3f, 0.3f);
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
    }
}


