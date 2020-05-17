using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Framework
{
    public class EasyAnimationState
    {
        readonly AnimationClipPlayable _playable;
        readonly string _stateName;
        readonly AnimationClip _clip;
        readonly WrapMode _wrapMode;
        public Playable Playable => _playable;
        public string StateName => _stateName;
        public AnimationClip Clip => _clip;
        public WrapMode WrapMode => _wrapMode;

        public float weight;
        public int index;

        public EasyAnimationState(AnimationClip clip, string stateName, PlayableGraph graph)
        {
            _stateName = stateName;
            _playable = AnimationClipPlayable.Create(graph, clip);
            _playable.SetApplyFootIK(false);
            _playable.SetApplyPlayableIK(false);
            if (!clip.isLooping || clip.wrapMode == WrapMode.Once)
            {
                _playable.SetDuration(clip.length);
            }
            _clip = clip;
            _wrapMode = clip.wrapMode;
        }

        public void Play()
        {
            _playable.Play();
        }

        public void Stop()
        {
            SetTime(0f);
        }

        public bool IsPlaying()
        {
            return !_playable.IsDone();
        }

        public float GetTime()
        {
            return (float) _playable.GetTime();
        }

        public void SetTime(float time)
        {
            _playable.SetTime(time);

            if (time >= _playable.GetDuration())
            {
                _playable.SetDone(true);
            }
        }
        
        public float GetSpeed()
        {
            return (float)_playable.GetSpeed();
        }
        
        public void SetSpeed(float speed)
        {
            _playable.SetSpeed(speed);
        }

        public void Destroy()
        {
            if (_playable.IsValid())
            {
                _playable.GetGraph().DestroySubgraph(_playable);
            }
        }

        public override string ToString()
        {
            return $"{_stateName}:{index}:{weight}:{WrapMode}:{_clip.length}";
        }
    }
    
    public class EasyAnimationStateCrossFade
    {
        readonly EasyAnimationState _state;
        readonly float _normalizedTransitionDuration;

        public EasyAnimationState State => _state;

        public EasyAnimationStateCrossFade(EasyAnimationState state, float normalizedTransitionDuration)
        {
            _state = state;
            _normalizedTransitionDuration = normalizedTransitionDuration;
        }

        public float UpdateWeight(float dt)
        {
            float otherWeight = 0f;
            
            if (MathHelper.EqualsZero(_normalizedTransitionDuration))
            {
                _state.weight = 1f;
                return otherWeight;
            }
            
            float oneFrameAddWeight = dt * (1f / _normalizedTransitionDuration);
            _state.weight += oneFrameAddWeight;
            otherWeight = 1 - _state.weight;

            if (_state.weight >= 1f)
            {
                _state.weight = 1f;
                otherWeight = 0f;
            }

            return otherWeight;
        }

        public bool IsFinish()
        {
            return _state.weight >= 1f;
        }
    }
}