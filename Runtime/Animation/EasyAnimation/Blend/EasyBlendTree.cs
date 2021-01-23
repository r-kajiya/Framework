using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;


namespace Framework
{
    [CreateAssetMenu]
    public sealed class EasyBlendTree : SerializedScriptableObject
    {
#if UNITY_EDITOR
        [OdinSerialize, InfoBox("Blend Tree can not be used at runtime")]
        UnityEditor.Animations.BlendTree _blendTree = null;

        [Button("Convert Runtime Format", ButtonSizes.Large)]
        void OnConvertRuntimeFormat()
        {
            _blendMotions.Clear();
            
            foreach (var childMotion in _blendTree.children)
            {
                var blendMotion = new EasyBlendMotion(
                    childMotion.motion,
                    childMotion.threshold,
                    childMotion.position,
                    childMotion.timeScale,
                    childMotion.cycleOffset,
                    childMotion.directBlendParameter,
                    childMotion.mirror);
                
                _blendMotions.Add(blendMotion);
            }

            _blendType = (EasyBlendTreeType) _blendTree.blendType;
            _blendParameter = _blendTree.blendParameter;
            _maxThreshold = _blendTree.maxThreshold;
            _minThreshold = _blendTree.minThreshold;
            _blendParameterY = _blendTree.blendParameterY;
            _useAutomaticThresholds = _blendTree.useAutomaticThresholds;
            _apparentSpeed = _blendTree.apparentSpeed;
            _averageDuration = _blendTree.averageDuration;
            _averageSpeed = _blendTree.averageSpeed;
            _isHumanMotion = _blendTree.isHumanMotion;
        }

#endif
        [OdinSerialize, ReadOnly]
        EasyBlendTreeType _blendType;
        public EasyBlendTreeType BlendTree => _blendType;
        
        [OdinSerialize, ReadOnly]
        string _blendParameter;
        public string BlendParameter => _blendParameter;

        [OdinSerialize, ReadOnly]
        float _maxThreshold;
        
        [OdinSerialize, ReadOnly]
        float _minThreshold;
        
        [OdinSerialize, ReadOnly]
        string _blendParameterY;
        
        [OdinSerialize, ReadOnly]
        bool _useAutomaticThresholds;
        
        [OdinSerialize, ReadOnly]
        float _apparentSpeed;
        
        [OdinSerialize, ReadOnly]
        float _averageDuration;
        
        [OdinSerialize, ReadOnly]
        Vector3 _averageSpeed;
        
        [OdinSerialize, ReadOnly]
        bool _isHumanMotion;
        
        [OdinSerialize, ReadOnly]
        List<EasyBlendMotion> _blendMotions = new List<EasyBlendMotion>();
        
        public List<EasyBlendMotion> BlendMotions => _blendMotions;
    }
}

