using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Framework
{
    public class AnimationEventObject : SerializedScriptableObject
    {
        [OdinSerialize, ReadOnly]
        int _frame;

        public int Frame => _frame;

        public float Time
        {
            get
            {
                float time = _frame * 0.01f;
                return time;
            }
        }

        [OdinSerialize]
        AnimationEventType _animationEventType;

        public AnimationEventType AnimationEventType => _animationEventType;

        [OdinSerialize]
        GameObject _effectPrefab;

        public GameObject EffectPrefab => _effectPrefab;

        [OdinSerialize]
        string _effectParentName;
        
        public string EffectParentName => _effectParentName;

        public static AnimationEventObject Create(int frame)
        {
            var obj = ScriptableObject.CreateInstance<AnimationEventObject>();
            obj._frame = frame;
            return obj;
            
        }
    }
}