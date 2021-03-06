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
        public string StateName { get; }

        public EasyAnimationStateBlend(EasyAnimationStateManager stateManager, EasyBlendTree tree, PlayableGraph graph, AnimationMixerPlayable mixer)
        {
            StateName = tree.name;

            for (int i = 0; i < tree.BlendMotions.Count; i++)
            {
                var clip = tree.BlendMotions[i].Motion as AnimationClip;
                string animationName = tree.name + "_" + clip.name + "_" + i;
                var state = stateManager.Find(animationName);
                _maps.Add(new Map(state, tree.BlendMotions[i]));
            }
        }

        public void Play()
        {
            foreach (var map in _maps)
            {
                map.State.SetTime(0f);
                map.State.Play();
                map.State.isBlending = true;
            }
        }

        public void ComputeDestinationWeights(float horizontal, float vertical)
        {
            float totalWeight = 0.0f;
            Vector2 point = new Vector2(horizontal, vertical);

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

                x.State.destinationWeight = stateWeight;
                totalWeight += stateWeight;
            }

            foreach (var map in _maps)
            {
                map.State.destinationWeight = map.State.destinationWeight / totalWeight;
            }
        }
    }
}