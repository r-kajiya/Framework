using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;

namespace Framework
{
    [RequireComponent(typeof(Animator))]
    public class EasyAnimation : MonoBehaviour
    {
        [SerializeField]
        bool _playAutomaticallyOnAwake = false;
        
        [SerializeField]
        bool _playAutomaticallyOnInitialize = false;
        
        [SerializeField]
        List<AnimationClip> _animationClips = null;

        [SerializeField]
        EasyBlendTree[] _blendTrees = null;

        [SerializeField, Range(0, 3), OnValueChanged("OnChangedAnimationSpeed")]
        float _animationSpeed = 1.0f;

        EasyAnimationPlayable _playable;
        public EasyAnimationPlayable Playable => _playable;
        PlayableGraph _playableGraph;
        Animator _animator;
        float _speedTemp;
        Vector3 _rootAnimationDeltaPosition;
        Quaternion _rootAnimationDeltaRotation;
        bool _isInitialized;

        public Animator Animator => _animator;
        public List<AnimationClip> AnimationClips => _animationClips;
        public Vector3 RootAnimationDeltaPosition => _rootAnimationDeltaPosition;
        public Quaternion RootAnimationDeltaRotation => _rootAnimationDeltaRotation;

        void Awake()
        {
            if (_playAutomaticallyOnAwake)
            {
                Initialize();
            }
        }

        public void Initialize()
        {
            if (_isInitialized)
            {
                DebugLog.Warning("EasyAnimation:Initialize:初期化済みです。", DebugLogColor.animation);
                return;
            }

            ForceInitialize();
        }

        public void ForceInitialize()
        {
            _isInitialized = true;
            _animator = GetComponent<Animator>();

            _playableGraph = PlayableGraph.Create();
            _playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            var template = new EasyAnimationPlayable();
            var playable = ScriptPlayable<EasyAnimationPlayable>.Create(_playableGraph, template, 1);
            _playable = playable.GetBehaviour();

            foreach (var animationClip in _animationClips)
            {
                _playable.Add(animationClip, animationClip.name);
            }

            foreach (var blendTree in _blendTrees)
            {
                _playable.Add(blendTree);
            }
            
            foreach (var blendTree in _blendTrees)
            {
                foreach (var blendMotion in blendTree.BlendMotions)
                {
                    var animationClip = blendMotion.Motion as AnimationClip;

                    if (!_animationClips.Contains(animationClip))
                    {
                        _animationClips.Add(animationClip);
                    }
                }
            }

            AnimationPlayableUtilities.Play(_animator, _playable.Self, _playableGraph);

            if (_playAutomaticallyOnInitialize)
            {
                _playable.Play(0);   
            }
        }

        public void Release()
        {
            if (_playableGraph.IsValid())
            {
                _playableGraph.Destroy();   
            }
        }

        public bool Add(AnimationClip clip, string stateName)
        {
            return _playable.Add(clip, stateName);
        }

        public bool Remove(string stateName)
        {
            return _playable.Remove(stateName);
        }
        
        public bool Play(int stateIndex, float time = 0f, float transitionTime = 0.3f)
        {
            return _playable.Play(stateIndex, time, transitionTime);
        }

        public bool Play(string stateName, float time = 0f, float transitionTime = 0.3f)
        {
            return _playable.Play(stateName, time, transitionTime);
        }

        public bool Blend(string blendTreeName, float transitionTime = 0.3f)
        {
            return _playable.Blend(blendTreeName, transitionTime);
        }

        public bool IsBlending(string blendTreeName)
        {
            return _playable.IsBlending(blendTreeName);
        }
        
        public bool SetBlendParameter(string blendTreeName, float horizontal, float vertical)
        {
            return _playable.SetBlendParameter(blendTreeName, horizontal, vertical);
        }
        
        public bool IsPlaying(string stateName)
        {
            return _playable.IsPlaying(stateName);
        }
        
        public bool IsPlaying(int stateIndex)
        {
            return _playable.IsPlaying(stateIndex);
        }
        
        public bool IsAnythingPlaying()
        {
            return _playable.IsAnythingPlaying();
        }

        public void SetSpeed(float speed)
        {
            _playable.SetSpeed(speed);
        }
        
        public float GetSpeed()
        {
            return _playable.GetSpeed();
        }

        public EasyAnimationState GetState(string stateName)
        {
            return _playable.GetState(stateName);
        }
        
        public EasyAnimationState GetCurrentState()
        {
            return _playable.GetPlayingState();
        }

        public void Stop()
        {
            _playable.Stop();
        }

        public void Pause()
        {
            _speedTemp = _playable.GetSpeed(); 
            _playable.SetSpeed(0);
        }
        
        public void Resume()
        {
            if (MathHelper.EqualsZero(_speedTemp))
            {
                DebugLog.Warning($"EasyAnimation.Resume : Pause関数を呼んでいません", DebugLogColor.animation);
                return;
            }
            
            _playable.SetSpeed(_speedTemp);
        }
        
        public void HasPlayable()
        {}
        

        public override string ToString()
        {
            string output = string.Empty;
            output += $"{gameObject.name}=";
            output += $"AnimationCount:{_playable.GetAllState()},";
            output += $"Speed:{GetSpeed()},";
            output += $"States:";

            foreach (var state in _playable.GetAllState())
            {
                output += $"{state.StateName}:{state.index}:{state.weight}:{state.WrapMode}:IsPlaying={state.IsPlaying()}";   
            }

            return output;
        }
        
        void OnDestroy()
        {
            if (_playableGraph.IsValid())
            {
                Release();
            }
        }

        void OnAnimatorMove()
        {
            if (Animator == null)
            {
                return;
            }

            _rootAnimationDeltaPosition = Animator.deltaPosition;
            _rootAnimationDeltaRotation = Animator.deltaRotation;

            // float angleInDegrees;
            // Vector3 rotationAxis;
            // animDeltaRotation.ToAngleAxis(out angleInDegrees, out rotationAxis);
            //
            // Vector3 angularDisplacement = rotationAxis * angleInDegrees * Mathf.Deg2Rad;
            // Vector3 animAngularVelocity = angularDisplacement / Time.deltaTime;
        }
        
#if UNITY_EDITOR
        void OnChangedAnimationSpeed()
        {
            SetSpeed(_animationSpeed);
        }
#endif
    }
}

