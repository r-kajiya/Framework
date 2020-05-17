using UnityEngine;
using UnityEngine.Playables;

namespace Framework
{
    [RequireComponent(typeof(Animator))]
    public class EasyAnimation : MonoBehaviour
    {
        [SerializeField]
        bool _playAutomatically = false;
        
        [SerializeField]
        AnimationClip[] _animationClips = null;

        EasyAnimationPlayable _playable;
        PlayableGraph _playableGraph;
        Animator _animator;
        float _speedTemp;

        public Animator Animator
        {
            get
            {
                return _animator;
            }
        }

        public void Initialize()
        {
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

            AnimationPlayableUtilities.Play(_animator, _playable.Self, _playableGraph);

            if (_playAutomatically)
            {
                _playable.Play(0, 0);   
            }
        }

        public void Release()
        {
            _playableGraph.Destroy();
        }

        public bool Add(AnimationClip clip, string stateName)
        {
            return _playable.Add(clip, stateName);
        }

        public bool Remove(string stateName)
        {
            return _playable.Remove(stateName);
        }
        
        public bool Play(int stateIndex, float time)
        {
            return _playable.Play(stateIndex, time);
        }

        public bool Play(string stateName, float time)
        {
            return _playable.Play(stateName, time);
        }
        
        public bool PlayNormalizeTime(int stateIndex, float normalizeTime)
        {
            var state = _playable.GetState(stateIndex);
            if (state == null)
            {
                return false;
            }
            
            float time = state.Clip.length * normalizeTime;
            return _playable.Play(stateIndex, time);
        }
        
        public bool PlayNormalizeTime(string stateName, float normalizeTime)
        {
            var state = _playable.GetState(stateName);
            if (state == null)
            {
                return false;
            }
            
            float time = state.Clip.length * normalizeTime;
            return _playable.Play(stateName, time);
        }
        
        public bool CrossFade(int stateIndex, float time, float normalizedTransitionDuration)
        {
            return _playable.CrossFade(stateIndex, time, normalizedTransitionDuration);
        }

        public bool CrossFade(string stateName, float time, float normalizedTransitionDuration)
        {
            return _playable.CrossFade(stateName, time, normalizedTransitionDuration);
        }
        
        public bool IsPlaying(string stateName)
        {
            return _playable.IsPlaying(stateName);
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

        public void Pause()
        {
            _speedTemp = _playable.GetSpeed(); 
            _playable.SetSpeed(0);
        }
        
        public void Resume()
        {
            if (MathHelper.EqualsZero(_speedTemp))
            {
                DebugLog.Warning($"EasyAnimation.Resume : Pause関数を呼んでいません");
                return;
            }
            
            _playable.SetSpeed(_speedTemp);
        }

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
    }
}

