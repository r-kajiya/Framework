#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace AnimalKing
{
    [RequireComponent(typeof(EasyAnimation))]
    [RequireComponent(typeof(Animator))]
    public class EasyAnimationAwakePlay : MonoBehaviour
    {
        [Serializable]
        public class AnimationButton
        {
            int _index;
            EasyAnimation _animation;
            
            public AnimationButton(int index, EasyAnimation animation)
            {
                _index = index;
                _animation = animation;
            }
            
            [Button]
            public void OnAction()
            {
                _animation.Play(_index, 0.0f);
            }
        }
        
        EasyAnimation _animation;

        EasyAnimation Animation
        {
            get
            {
                if (_animation == null)
                {
                    _animation = GetComponent<EasyAnimation>();
                }
                return _animation;
            }
        }

        [TableList(AlwaysExpanded = true)]
        public List<AnimationButton> _animationButton = new List<AnimationButton>();

        void Start()
        {
            Animation.Initialize();

            foreach (var clip in Animation.AnimationClips)
            {
                var state = Animation.GetState(clip.name);
                _animationButton.Add(new AnimationButton(state.index, _animation));
            }
        }
    }
}
#endif