using System;
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
        float _transitionTime = 0.3f;
        string _blendingName;
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
            UpdateTransition(data.deltaTime);
        }

        public bool Add(AnimationClip clip, string stateName)
        {
            return _stateManager.Add(clip, stateName, _graph, _mixer);
        }
        
        public bool Add(EasyBlendTree blendTree)
        {
            return _stateManager.AddBlend(blendTree, _graph, _mixer);
        }

        public bool Remove(string stateName)
        {
            if (_stateManager.Remove(stateName))
            {
                return true;
            }

            return _stateManager.RemoveBlend(stateName);
        }

        public bool Play(int index, float time = 0.0f, float transitionTime = 0.3f)
        {
            var target = _stateManager.Find(index);
            if (target == null)
            {
                DebugLog.Error($"EasyAnimationPlayable.Play : アニメーションステートが存在しないため再生できませんでした。{index}");
                return false;
            }

            return Play(target.StateName, time, transitionTime);
        }

        public bool Play(string stateName, float time = 0.0f, float transitionTime = 0.3f)
        {
            if (!_stateManager.Exists(stateName))
            {
                DebugLog.Error($"EasyAnimationPlayable.Play : アニメーションステートが存在しないため再生できませんでした。{stateName}");
                return false;
            }

            _blendingName = string.Empty;
            _transitionTime = transitionTime;

            foreach (var state in _stateManager.states)
            {
                state.originWeight = state.weight;
                
                if (state.StateName == stateName)
                {
                    state.destinationWeight = 1f;
                    state.SetTime(time);
                    state.Play();
                }
                else
                {
                    state.destinationWeight = 0f;
                    state.isBlending = false;
                }
            }

            DebugLog.Normal($"EasyAnimationPlayable.Play : {stateName}を再生します");

            return true;
        }

        public bool Blend(string blendTreeName, float transitionTime = 0.3f)
        {
            var target = _stateManager.FindBlend(blendTreeName);
            if (target == null)
            {
                DebugLog.Error($"EasyAnimationPlayable.Blend : ブレンドアニメーションステートが存在しないため再生できませんでした。{blendTreeName}");
                return false;
            }
            
            _blendingName = blendTreeName;
            _transitionTime = transitionTime;

            foreach (var state in _stateManager.states)
            {
                state.destinationWeight = 0f;
                state.originWeight = state.weight;
                state.isBlending = false;
            }

            target.Play();
            target.ComputeDestinationWeights(0f, 0f);

            DebugLog.Normal($"EasyAnimationPlayable.Blend : {blendTreeName}を再生します。");

            return true;
        }

        public bool SetBlendParameter(string blendTreeName, float horizontal, float vertical)
        {
            if (_blendingName != blendTreeName)
            {
                DebugLog.Warning($"EasyAnimationPlayable.SetBlendParameter : ブレンドアニメーション中じゃありません。{blendTreeName}");
                return false;
            }
            
            var blend = _stateManager.FindBlend(blendTreeName);
            
            if (blend == null)
            {
                DebugLog.Warning($"EasyAnimationPlayable.SetBlendParameter : 存在しないブレンドアニメーションです。{blendTreeName}");
                return false;
            }
            
            blend.ComputeDestinationWeights(horizontal, vertical);
            
            foreach (var state in _stateManager.states)
            {
                state.originWeight = state.weight;
            }

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
            if (state != null)
            {
                return state.IsPlaying();
            }

            return false;
        }
        
        public bool IsBlending(string blendTreeName)
        {
            return _blendingName == blendTreeName;
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
            foreach (var state in _stateManager.states)
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

            foreach (var state in _stateManager.states)
            {
                state.SetSpeed(speed);
            }
        }

        public float GetSpeed()
        {
            return _stateManager.states[0].GetSpeed();
        }

        public void Stop()
        {
            DebugLog.Normal($"EasyAnimationPlayable.Stop : 停止します");

            foreach (var state in _stateManager.states)
            {
                state.Stop();
            }

            _blendingName = string.Empty;
        }

        public EasyAnimationState GetPlayingState()
        {
            foreach (var state in _stateManager.states)
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

            foreach (var state in _stateManager.states)
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
            return _stateManager.states;
        }

        void UpdateTransition(float dt)
        {
            if (_transitionTime.EqualsZero())
            {
                _transitionTime = 0.001f;
            }

            if (dt.EqualsZero())
            {
                dt = 0.0333f;
            }

            float totalWeight = 0f;
            
            foreach (var state in _stateManager.states)
            {
                float defaultFrameAddWeight = dt * (1f / _transitionTime);
                float diff = state.destinationWeight - state.weight;
                float frameAddWeight = 0f;
                
                if (!diff.EqualsZero())
                {
                    frameAddWeight = defaultFrameAddWeight / diff;
                }

                float previousWeight = state.weight;
                state.weight += frameAddWeight;

                if (state.isBlending)
                {
                    int a = Math.Sign(previousWeight - state.destinationWeight);
                    int b = Math.Sign(state.weight - state.destinationWeight);

                    if (a != 0 && b != 0)
                    {
                        if (a != b)
                        {
                            state.weight = state.destinationWeight;
                        }   
                    }
                }
                else
                {
                    if (state.weight >= 1f)
                    {
                        state.weight = 1f;
                    }
                    else if (state.weight <= 0f)
                    {
                        state.weight = 0f;
                        state.Stop();
                    }
                }

                totalWeight += state.weight;

                _mixer.SetInputWeight(state.index, state.weight);
            }

            if (totalWeight > 1.0f)
            {
                foreach (var state in _stateManager.states)
                {
                    if (state.weight.EqualsZero())
                    {
                        continue;
                    }
                    
                    state.weight = state.weight / totalWeight;
                    _mixer.SetInputWeight(state.index, state.weight);
                }
            }
        }
    }
}