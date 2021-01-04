using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Framework
{
    [CreateAssetMenu]
    public sealed class AnimationEvents : SerializedScriptableObject, IEnumerable<AnimationClipEvents>
    {
        [OdinSerialize]
        [ListDrawerSettings( HideAddButton = true, HideRemoveButton = true )]
        List<AnimationClipEvents> _animationClipEventsList = new List<AnimationClipEvents>();
        public List<AnimationClipEvents> AnimationClipEventsList => _animationClipEventsList;

        public IEnumerator<AnimationClipEvents> GetEnumerator()
        {
            return _animationClipEventsList.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
#if UNITY_EDITOR

        public AnimationEventInvoker AnimationEventInvokerOnScene { get; private set; }

        public AnimationClipEvents AddDefault()
        {
            var obj = ScriptableObject.CreateInstance<AnimationClipEvents>();
            _animationClipEventsList.Add(obj);
            AssetDatabase.AddObjectToAsset(obj, this);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(this));
            return obj;
        }
        
        public void RemoveAt(int index)
        {
            _animationClipEventsList.RemoveAt(index);
        }
#endif
    }
}


