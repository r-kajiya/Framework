using System;
using Sirenix.Serialization;
using UnityEngine;

namespace Framework
{
    [Serializable]
    public sealed class EasyBlendMotion
    {
        public EasyBlendMotion(
            Motion motion, 
            float threshold, 
            Vector2 position,
            float timeScale,
            float cycleOffset,
            string directBlendParameter,
            bool mirror)
        {
            _motion = motion;
            _threshold = threshold;
            _position = position;
            _timeScale = timeScale;
            _cycleOffset = cycleOffset;
            _directBlendParameter = directBlendParameter;
            _mirror = mirror;
        }
        
        [OdinSerialize]
        Motion _motion;
        public Motion Motion => _motion;
        
        [OdinSerialize]
        float _threshold;
        public float Threshold => _threshold;
        
        [OdinSerialize]
        Vector2 _position;
        public Vector2 Position => _position;
        
        [OdinSerialize]
        float _timeScale;
        public float TimeScale => _timeScale;
        
        [OdinSerialize]
        float _cycleOffset;
        public float CycleOffset => _cycleOffset;
        
        [OdinSerialize]
        string _directBlendParameter;
        public string DirectBlendParameter => _directBlendParameter;
        
        [OdinSerialize]
        bool _mirror;
        public bool Mirror => _mirror;
    }
}