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
            Vector2 position)
        {
            _motion = motion;
            _position = position;
        }
        
        [OdinSerialize]
        Motion _motion;
        public Motion Motion => _motion;

        [OdinSerialize]
        Vector2 _position;
        public Vector2 Position => _position;
        
    }
}