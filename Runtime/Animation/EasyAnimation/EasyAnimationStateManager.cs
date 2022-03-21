using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Framework
{
    public class EasyAnimationStateManager
    {
        public List<EasyAnimationState> states = new List<EasyAnimationState>();

        public List<EasyAnimationStateBlend> blends = new List<EasyAnimationStateBlend>();

        public EasyAnimationState Find(string stateName)
        {
            return states.Find(x => x.StateName == stateName);
        }

        public EasyAnimationState Find(int stateIndex)
        {
            return states.Find(x => x.index == stateIndex);
        }

        public EasyAnimationStateBlend FindBlend(string treeName)
        {
            return blends.Find(x => x.StateName == treeName);
        }

        public bool Exists(string stateName)
        {
            return states.Exists(x => x.StateName == stateName);
        }

        public bool Add(AnimationClip clip, string stateName, PlayableGraph graph, AnimationMixerPlayable mixer)
        {
            var findState = Find(stateName);
            if (findState == null)
            {
                var addState = new EasyAnimationState(clip, stateName, graph);
                states.Add(addState);
                addState.Stop();
                addState.index = states.Count - 1;
                int inputCount = addState.index + 1;
                mixer.SetInputCount(inputCount);
                graph.Connect(addState.Playable, 0, mixer, addState.index);
                DebugLog.Normal($"EasyAnimationStateManager.Add : アニメーションステートを追加しました。{clip.name}", DebugLogColor.animation);
                return true;
            }
            
            DebugLog.Warning($"EasyAnimationStateManager.Add : 同名アニメーションステートが存在しているため、追加に失敗しました。{clip.name}", DebugLogColor.animation);
            return false;
        }

        public bool AddBlend(EasyBlendTree tree, PlayableGraph graph, AnimationMixerPlayable mixer)
        {
            var findBlend = FindBlend(tree.name);
            if (findBlend == null)
            {
                for (int i = 0; i < tree.BlendMotions.Count; i++)
                {
                    var clip = tree.BlendMotions[i].Motion as AnimationClip;
                    string animationName = tree.name + "_" + clip.name + "_" + i;
                    Add(clip, animationName, graph, mixer);
                }

                blends.Add(new EasyAnimationStateBlend(this, tree, graph, mixer));
                DebugLog.Normal($"EasyAnimationStateManager.AddBlend : ブレンドアニメーションステートを追加しました。{tree.name}", DebugLogColor.animation);
                return true;
            }

            DebugLog.Warning($"EasyAnimationStateManager.AddBlend : 同名ブレンドアニメーションステートが存在しているため、追加に失敗しました。{tree.name}", DebugLogColor.animation);
            return false;
        }

        public bool Remove(string removeStateName)
        {
            var findState = Find(removeStateName);
            if (findState != null)
            {
                findState.Destroy();
                states.Remove(findState);
                return true;
            }

            DebugLog.Warning($"EasyAnimationStateManager.Remove : アニメーションステートが存在していないため、追加に失敗しました。{removeStateName}", DebugLogColor.animation);
            return false;
        }

        public bool RemoveBlend(string removeStateName)
        {
            var findState = FindBlend(removeStateName);
            if (findState != null)
            {
                blends.Remove(findState);
                DebugLog.Normal($"EasyAnimationStateManager.RemoveBlend : ブレンドアニメーションステートを削除しました。{removeStateName}", DebugLogColor.animation);
                return true;
            }

            DebugLog.Warning($"EasyAnimationStateManager.RemoveBlend : ブレンドアニメーションステートが存在していないため、追加に失敗しました。{removeStateName}", DebugLogColor.animation);
            return false;
        }
    }
}