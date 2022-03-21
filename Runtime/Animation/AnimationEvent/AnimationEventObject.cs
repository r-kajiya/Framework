using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Framework
{
    public class AnimationEventObject : SerializedScriptableObject
    {
        [OdinSerialize]
        AnimationEventObjectExecutor _executor;
        public AnimationEventObjectExecutor Executor => _executor;
        
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

        [OdinSerialize, ReadOnly]
        AnimationClipEvents _parent;
        protected AnimationClipEvents Parent => _parent;

        public string AnimationEventName => _executor.GetType().Name;
        
#if UNITY_EDITOR

        public static AnimationEventObject Create(int frame, AnimationClipEvents parent)
        {
            AnimationEventObject obj = CreateInstance<AnimationEventObject>();
            
            obj._frame = frame;
            obj._parent = parent;
            
            return obj;
        }
#endif
        
    }
    
// #if UNITY_EDITOR
//     [CustomEditor(typeof(AnimationEventObject))]
//     public class AnimationEventObjectEditor : OdinEditor{
//         
//         SerializedScriptableObject _target;
//         
//         public override void OnInspectorGUI() {
//             _target = EditorGUILayout.ObjectField( _target, typeof(AnimationEventObject), false) as AnimationEventObject;
//             
//             base.OnInspectorGUI();
//         }
//     }
// #endif
}