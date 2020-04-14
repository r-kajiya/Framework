using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Framework
{
    [RequireComponent(typeof(Animator))]
    public class EasyAnimation : MonoBehaviour
    {
        [SerializeField]
        AnimationClip[] _animationClips;

        PlayableGraph _playableGraph;
        
        Animator _animator;
        public Animator Animator
        {
            get
            {
                if (_animator == null)
                {
                    _animator = GetComponent<Animator>();
                }

                return _animator;
            }
        }

        void Start()
        {
            _playableGraph = PlayableGraph.Create();
            _playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            var playableOutput = AnimationPlayableOutput.Create(_playableGraph, "Animation", Animator);
            var clipPlayable = AnimationClipPlayable.Create(_playableGraph, _animationClips[0]);
            playableOutput.SetSourcePlayable(clipPlayable);
            _playableGraph.Play();
        }

        void OnDestroy()
        {
            _playableGraph.Destroy();
        }

        public bool Play(string stateName)
        {
            return true;
        }

        public void Blend()
        {
            
        }

        public void CrossFade()
        {
            
        }
    }
}

