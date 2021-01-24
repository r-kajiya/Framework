using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using Vector2 = UnityEngine.Vector2;

namespace Framework
{
    public class EasyAnimationStateBlend
    {
        class Map
        {
            readonly EasyAnimationState _state;
            readonly EasyBlendMotion _blend;
            public EasyAnimationState State => _state;
            public EasyBlendMotion Blend => _blend;

            public Map(EasyAnimationState state, EasyBlendMotion blend)
            {
                _state = state;
                _blend = blend;
            }
        }

        readonly List<Map> _maps = new List<Map>();
        float _pointTransitionDuration = 0.3f;
        float _normalizedTransitionDuration = 0.3f;
        public float NormalizedTransitionDuration => _normalizedTransitionDuration;
        Destination2D _destinationPoint;
        readonly string _stateName;
        public string StateName => _stateName;
        public float weight;
        public int MotionCount => _maps.Count;

        public EasyAnimationStateBlend(EasyBlendTree tree, PlayableGraph graph, AnimationMixerPlayable mixer,
            int startIndex)
        {
            _stateName = tree.name;

            for (int i = 0; i < tree.BlendMotions.Count; i++)
            {
                var blendMotion = tree.BlendMotions[i];
                var clip = blendMotion.Motion as AnimationClip;
                string motionName = _stateName + "_" + clip.name;
                EasyAnimationState state = new EasyAnimationState(clip, motionName, graph);
                state.index = startIndex + i;
                _maps.Add(new Map(state, blendMotion));
                mixer.SetInputCount(state.index + 1);
                graph.Connect(state.Playable, 0, mixer, state.index);
                state.Stop();
            }
        }

        public void Play(float normalizedTransitionDuration, float pointTransitionDuration)
        {
            _pointTransitionDuration = pointTransitionDuration;
            _normalizedTransitionDuration = normalizedTransitionDuration;

            foreach (var map in _maps)
            {
                map.State.SetTime(0f);
                map.State.Play();
            }

            SetPoint(0f, 0f);
            weight = 0f;
        }

        public bool IsPlaying()
        {
            foreach (var map in _maps)
            {
                if (map.State.IsPlaying())
                {
                    return true;
                }
            }

            return false;
        }

        public void SetSpeed(float speed)
        {
            foreach (var map in _maps)
            {
                map.State.SetSpeed(speed);
            }
        }

        public float GetSpeed()
        {
            return _maps[0].State.GetSpeed();
        }

        public void Stop(AnimationMixerPlayable mixer)
        {
            foreach (var map in _maps)
            {
                map.State.weight = 0;
                map.State.Stop();
                mixer.SetInputWeight(map.State.index, map.State.weight);
            }

            weight = 0f;
        }


        public void SetPoint(float horizontal, float vertical)
        {
            var destination = new Vector2(horizontal, vertical);
            _destinationPoint.Start(_destinationPoint.Current(), destination, _pointTransitionDuration);
        }

        public void UpdateWeight(AnimationMixerPlayable mixer, float dt)
        {
            _destinationPoint.Update(dt);
            Vector2 point = _destinationPoint.Current();

            float totalWeight = 0.0f;

            foreach (var x in _maps)
            {
                Vector2 pointX = x.Blend.Position;
                Vector2 pointToPointX = point - pointX;
                float stateWeight = 1.0f;

                foreach (var y in _maps)
                {
                    if (x.Blend.Motion.name == y.Blend.Motion.name)
                    {
                        continue;
                    }

                    Vector2 pointY = y.Blend.Position;
                    Vector2 pointYToPointX = pointY - pointX;
                    float pointYToPointXLen = Vector2.Dot(pointYToPointX, pointYToPointX);
                    float newWeight = Vector2.Dot(pointToPointX, pointYToPointX) / pointYToPointXLen;
                    newWeight = 1.0f - newWeight;
                    newWeight = Mathf.Clamp(newWeight, 0.0f, 1.0f);
                    stateWeight = Mathf.Min(stateWeight, newWeight);
                }

                x.State.weight = stateWeight;
                totalWeight += stateWeight;
            }

            foreach (var map in _maps)
            {
                map.State.weight = (map.State.weight / totalWeight) * weight;
                mixer.SetInputWeight(map.State.index, map.State.weight);
            }
        }

        public float UpdateTransitionWeight(float dt)
        {
            float otherWeight = 0f;

            if (MathHelper.EqualsZero(_normalizedTransitionDuration))
            {
                weight = 1f;
                return otherWeight;
            }

            float oneFrameAddWeight = dt * (1f / _normalizedTransitionDuration);
            weight += oneFrameAddWeight;
            otherWeight = 1 - weight;

            if (weight >= 1f)
            {
                weight = 1f;
                otherWeight = 0f;
            }

            return otherWeight;
        }

        public bool IsTransitionFinish()
        {
            return weight >= 1f;
        }
    }
}