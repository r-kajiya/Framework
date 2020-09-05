using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

namespace Framework
{
    public class EasyAnimationPlayable : PlayableBehaviour
    {
        Playable _self;
        AnimationMixerPlayable _mixer;
        PlayableGraph _graph;
        List<EasyAnimationState> _pauseStates;
        
        readonly EasyAnimationStateManager _stateManager;

        public Playable Self => _self;

        public EasyAnimationPlayable()
        {
            _stateManager = new EasyAnimationStateManager();
        }

        public override void OnPlayableCreate(Playable playable)
        {
            _self = playable;
            _self.SetInputCount(1);
            _self.SetInputWeight(0, 1);
            _mixer = AnimationMixerPlayable.Create(_self.GetGraph(), 1, true);
            _graph = _self.GetGraph();
            _graph.Connect(_mixer, 0, _self, 0);
            _graph.Play();
        }
        
        public override void PrepareFrame(Playable owner, FrameData data)
        {
            UpdateCrossFade(data.deltaTime);
        }

        public bool Add(AnimationClip clip, string stateName)
        {
            EasyAnimationState newState = new EasyAnimationState(clip, stateName, _graph);
            if (!_stateManager.Add(newState))
            {
                return false;
            }
            
            newState.index = _stateManager.Count() - 1;
            int inputCount = newState.index + 1;
            _mixer.SetInputCount(inputCount);
            _graph.Connect(newState.Playable, 0, _mixer, newState.index);

            return true;
        }

        public bool Remove(string stateName)
        {
            return _stateManager.Remove(stateName);
        }

        public bool Play(int stateIndex, float time)
        {
            var target = _stateManager.Find(stateIndex);
            if (target == null)
            {
                DebugLog.Error($"EasyAnimationPlayable.Play : アニメーションステートが存在しないため再生できませんでした。{stateIndex}");
                return false;
            }

            return Play(target.StateName, time);
        }

        public bool Play(string stateName, float time)
        {
            if (!_stateManager.Exists(stateName))
            {
                DebugLog.Error($"EasyAnimationPlayable.Play : アニメーションステートが存在しないため再生できませんでした。{stateName}");
                return false;
            }

            foreach (var state in _stateManager.States)
            {
                if (state.StateName == stateName)
                {
                    state.SetTime(time);
                    state.weight = 1;
                    state.Play();
                    _mixer.SetInputWeight(state.index, state.weight);
                }
                else
                {
                    state.weight = 0;
                    state.Stop();
                    _mixer.SetInputWeight(state.index, state.weight);
                }
            }
            
            DebugLog.Normal($"EasyAnimationPlayable.Play : {stateName}を再生します");

            return true;
        }
        
        public bool CrossFade(int stateIndex, float time, float normalizedTransitionDuration)
        {
            var target = _stateManager.Find(stateIndex);
            if (target == null)
            {
                DebugLog.Error($"EasyAnimationPlayable.CrossFade : アニメーションステートが存在しないため再生できませんでした。{stateIndex}");
                return false;
            }

            return CrossFade(target.StateName, time, normalizedTransitionDuration);
        }

        public bool CrossFade(string stateName, float time, float normalizedTransitionDuration)
        {
            var target = _stateManager.Find(stateName);
            if (target == null)
            {
                DebugLog.Error($"EasyAnimationPlayable.CrossFade : アニメーションステートが存在しないため再生できませんでした。{stateName}");
                return false;
            }
            
            target.SetTime(time);
            _stateManager.crossFadeTarget = new EasyAnimationStateCrossFade(target, normalizedTransitionDuration);

            return true;
        }

        public EasyAnimationState GetState(string stateName)
        {
            var target = _stateManager.Find(stateName);
            if (target == null)
            {
                DebugLog.Error($"EasyAnimationPlayable.GetState : アニメーションステートが存在しないため取得できませんでした。{stateName}");
                return null;
            }

            return target;
        }
        
        public EasyAnimationState GetState(int stateIndex)
        {
            EasyAnimationState target = _stateManager.Find(stateIndex);
            
            if (target == null)
            {
                DebugLog.Error($"EasyAnimationPlayable.GetState : アニメーションステートが存在しないため取得できませんでした。{stateIndex}");
                return null;
            }

            return target;
        }
        
        public bool IsPlaying(string stateName)
        {
            var state = _stateManager.Find(stateName);
            if (state == null)
            {
                return false;
            }
            
            return state.IsPlaying();
        }
        
        public bool IsPlaying(int stateIndex)
        {
            var state = _stateManager.Find(stateIndex);
            if (state == null)
            {
                return false;
            }
            
            return state.IsPlaying();
        }
        
        public bool IsAnythingPlaying()
        {
            foreach (var state in _stateManager.States)
            {
                if (state.IsPlaying())
                {
                    return true;
                }
            }
            
            return false;
        }

        public void SetSpeed(float speed)
        {
            DebugLog.Normal($"EasyAnimationPlayable.SetSpeed : 再生速度を{speed}にします");
            
            foreach (var state in _stateManager.States)
            {
                state.SetSpeed(speed);
            }
        }

        public float GetSpeed()
        {
            return _stateManager.States[0].GetSpeed();
        }

        public EasyAnimationState GetPlayingState()
        {
            foreach (var state in _stateManager.States)
            {
                if (state.IsPlaying())
                {
                    return state;
                }
            }

            return null;
        }
        
        public List<EasyAnimationState> GetPlayingStates()
        {
            List<EasyAnimationState> states = new List<EasyAnimationState>();
            
            foreach (var state in _stateManager.States)
            {
                if (state.IsPlaying())
                {
                    states.Add(state);
                }
            }

            return states;
        }

        public List<EasyAnimationState> GetAllState()
        {
            return _stateManager.States;
        }

        void UpdateCrossFade(float dt)
        {
            if (_stateManager.crossFadeTarget == null)
            {
                return;
            }

            var target = _stateManager.crossFadeTarget.State;
            float otherWeight = _stateManager.crossFadeTarget.UpdateWeight(dt);
            _mixer.SetInputWeight(target.index, target.weight);

            if (_stateManager.crossFadeTarget.IsFinish())
            {
                _stateManager.crossFadeTarget = null;
            }

            foreach (var state in _stateManager.States)
            {
                if (state.index == target.index)
                {
                    continue;
                }
                
                if (MathHelper.EqualsZero(state.weight))
                {
                    _mixer.SetInputWeight(state.index, 0);
                    continue;
                }
                
                state.weight = otherWeight;
                _mixer.SetInputWeight(state.index, state.weight);
            }
        }
        
    }
}