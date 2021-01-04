using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    public class AnimationClipEvents : SerializedScriptableObject
    {
        [OdinSerialize, ShowInInspector]
        AnimationClip _animationClip;
        public AnimationClip AnimationClip => _animationClip;

        [OdinSerialize]
        bool _isOverrideAnimationClipName;

        bool DisableOverrideAnimationClipName => !_isOverrideAnimationClipName;

        [OdinSerialize, DisableIf("DisableOverrideAnimationClipName")]
        string _overrideAnimationClipName;

        public string AnimationName
        {
            get
            {
                if (_isOverrideAnimationClipName)
                {
                    return _overrideAnimationClipName;
                }

                if (AnimationClip == null)
                {
                    return null;
                }

                return AnimationClip.name;
            }
        }

            [OdinSerialize]
        [DictionaryDrawerSettings( IsReadOnly = true, DisplayMode = DictionaryDisplayOptions.OneLine)]
        Dictionary<int, AnimationEventObject> _animationEventObjectMap = new Dictionary<int, AnimationEventObject>();
        public Dictionary<int, AnimationEventObject> AnimationEventObjectMap => _animationEventObjectMap;
        
#if UNITY_EDITOR
        public AnimationEventObject AddAt(int frame)
        {
            var obj = AnimationEventObject.Create(frame);
            AnimationEventObjectMap.Add(frame, obj);
            AssetDatabase.AddObjectToAsset(obj, this);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(this));
            return obj;
        }
        
        public void RemoveAt(int frame)
        {
            AnimationEventObjectMap.Remove(frame);
        }

        public bool TryGetValue(int frame, out AnimationEventObject value)
        {
            return AnimationEventObjectMap.TryGetValue(frame, out value);
        }

        public AnimationEventObject GetValue(int frame)
        {
            if (AnimationEventObjectMap.TryGetValue(frame, out var value))
            {
                return value;
            }

            return null;
        }

        public bool IsContainsKey(int frame)
        {
            return AnimationEventObjectMap.ContainsKey(frame);
        }
#endif
    }
}