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
            UpdateBlend(data.deltaTime);
        }

        public bool Add(AnimationClip clip, string stateName)
        {
            EasyAnimationState newState = new EasyAnimationState(clip, stateName, _graph);
            if (!_stateManager.Add(newState))
            {
                return false;
            }

            newState.Stop();
            newState.index = _stateManager.AllCount() - 1;
            int inputCount = newState.index + 1;
            _mixer.SetInputCount(inputCount);
            _graph.Connect(newState.Playable, 0, _mixer, newState.index);

            return true;
        }
        
        public bool Add(EasyBlendTree blendTree)
        {
            int startIndex = _stateManager.AllCount();

            if (!_stateManager.AddBlend(blendTree, _graph, _mixer, startIndex))
            {
                return false;
            }

            return true;
        }

        public bool Remove(string stateName)
        {
            if (_stateManager.Remove(stateName))
            {
                return true;
            }

            return _stateManager.RemoveBlend(stateName);
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

            BlendStopIfNeeded();

            foreach (var state in _stateManager.states)
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

            BlendStopIfNeeded();

            target.SetTime(time);
            target.Play();
            _stateManager.crossFadeTarget = new EasyAnimationStateCrossFade(target, normalizedTransitionDuration);

            return true;
        }

        public bool Blend(string stateName, float normalizedTransitionDuration, float pointTransitionDuration)
        {
            var target = _stateManager.FindBlend(stateName);
            if (target == null)
            {
                DebugLog.Error($"EasyAnimationPlayable.Blend : ブレンドアニメーションステートが存在しないため再生できませんでした。{stateName}");
                return false;
            }

            if (_stateManager.targetBlend?.StateName == target.StateName)
            {
                DebugLog.Warning($"EasyAnimationPlayable.Blend : 再生中のブレンドアニメーションステートを再生しようとしたためキャンセルしました。{stateName}");
                return false;
            }

            _stateManager.crossFadeTarget = null;

            target.Play(normalizedTransitionDuration, pointTransitionDuration);
            _stateManager.targetBlend = target;

            return true;
        }

        public bool SetBlendParameter(float horizontal, float vertical)
        {
            if (_stateManager.targetBlend == null)
            {
                return false;
            }

            _stateManager.targetBlend.SetPoint(horizontal, vertical);

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

            if (_stateManager.targetBlend == null)
            {
                return false;
            }

            if (_stateManager.targetBlend.IsPlaying())
            {
                return true;
            }

            return false;
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

            foreach (var blend in _stateManager.blends)
            {
                blend.SetSpeed(speed);
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
                foreach (var state in _stateManager.states)
                {
                    if (state.index == target.index)
                    {
                        continue;
                    }

                    state.Stop();
                    _mixer.SetInputWeight(state.index, 0);
                }

                foreach (var blend in _stateManager.blends)
                {
                    blend.Stop(_mixer);
                }

                _stateManager.crossFadeTarget = null;
            }

            foreach (var state in _stateManager.states)
            {
                if (state.index == target.index)
                {
                    continue;
                }

                if (state.weight.EqualsZero())
                {
                    state.Stop();
                    _mixer.SetInputWeight(state.index, 0);
                    continue;
                }

                state.weight = otherWeight;
                _mixer.SetInputWeight(state.index, state.weight);
            }

            foreach (var blend in _stateManager.blends)
            {
                if (!blend.weight.EqualsZero())
                {
                    blend.weight = otherWeight;
                    blend.UpdateWeight(_mixer, dt);
                }
            }
        }

        void UpdateBlend(float dt)
        {
            if (_stateManager.crossFadeTarget != null)
            {
                return;
            }
            
            if (_stateManager.targetBlend == null)
            {
                return;
            }

            _stateManager.targetBlend.UpdateWeight(_mixer, dt);

            if (!_stateManager.targetBlend.IsTransitionFinish())
            {
                float otherWeight = _stateManager.targetBlend.UpdateTransitionWeight(dt);

                foreach (var state in _stateManager.states)
                {
                    if (state.weight.EqualsZero())
                    {
                        state.Stop();
                        _mixer.SetInputWeight(state.index, 0);
                        continue;
                    }

                    state.weight = otherWeight;
                    _mixer.SetInputWeight(state.index, state.weight);
                }

                foreach (var blend in _stateManager.blends)
                {
                    if (blend.StateName == _stateManager.targetBlend.StateName)
                    {
                        continue;
                    }

                    if (!blend.weight.EqualsZero())
                    {
                        blend.UpdateWeight(_mixer, dt);
                    }
                }

                if (_stateManager.targetBlend.IsTransitionFinish())
                {
                    foreach (var state in _stateManager.states)
                    {
                        if (state.IsPlaying())
                        {
                            state.Stop();
                            _mixer.SetInputWeight(state.index, 0);
                        }
                    }

                    foreach (var blend in _stateManager.blends)
                    {
                        if (blend.StateName == _stateManager.targetBlend.StateName)
                        {
                            continue;
                        }

                        if (blend.IsPlaying())
                        {
                            blend.Stop(_mixer);
                        }
                    }
                }
            }
        }

        void BlendStopIfNeeded()
        {
            _stateManager.targetBlend = null;
        }
    }
}