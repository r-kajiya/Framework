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
        float _normalizedTransitionDuration = 0.3f;
        float _pointTransitionDuration = 0.3f;
        Destination2D _destinationPoint;
        Destination _destinationTransition;
        readonly string _stateName;
        public string StateName => _stateName;
        public float Weight => _destinationTransition.Current();

        public EasyAnimationStateBlend(List<EasyAnimationState> allStates, EasyBlendTree tree)
        {
            _stateName = tree.name;

            foreach (var state in allStates)
            {
                foreach (var blend in tree.BlendMotions)
                {
                    if (state.StateName == blend.Motion.name)
                    {
                        _maps.Add(new Map(state, blend));
                    }
                }
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
            
            _destinationTransition.Start(0.0f, 1.0f, _normalizedTransitionDuration);
        }

        public void Stop(AnimationMixerPlayable mixer)
        {
            foreach (var map in _maps)
            {
                map.State.weight = 0;
                map.State.Stop();
                mixer.SetInputWeight(map.State.index, map.State.weight);
            }
        }

        public void SetPoint(float horizontal, float vertical)
        {
            var destination = new Vector2(horizontal, vertical);
            _destinationPoint.Start(_destinationPoint.Current(), destination, _pointTransitionDuration);
        }

        public void UpdateWeight(AnimationMixerPlayable mixer, float dt, float blendWeight)
        {
            _destinationTransition.Update(dt);
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
                map.State.weight = map.State.weight / totalWeight * blendWeight;
                mixer.SetInputWeight(map.State.index, map.State.weight);
            }
        }
    }
}