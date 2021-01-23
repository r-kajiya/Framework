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
                    childMotion.position);
                
                _blendMotions.Add(blendMotion);
            }
        }

#endif

        [OdinSerialize, ReadOnly]
        List<EasyBlendMotion> _blendMotions = new List<EasyBlendMotion>();
        
        public List<EasyBlendMotion> BlendMotions => _blendMotions;
    }
}

