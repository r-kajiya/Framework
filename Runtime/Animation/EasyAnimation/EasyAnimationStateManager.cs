using System.Collections.Generic;

namespace Framework
{
    public class EasyAnimationStateManager
    {
        public List<EasyAnimationState> states = new List<EasyAnimationState>();
        
        public List<EasyAnimationStateBlend> blends = new List<EasyAnimationStateBlend>();

        public EasyAnimationStateCrossFade crossFadeTarget;

        public EasyAnimationStateBlend targetBlend;

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

        public bool Add(EasyAnimationState addState)
        {
            var findState = Find(addState.StateName);
            if (findState == null)
            {
                states.Add(addState);
                DebugLog.Normal($"EasyAnimationStateManager.Add : アニメーションステートを追加しました。{addState}");
                return true;
            }
            
            DebugLog.Warning($"EasyAnimationStateManager.Add : アニメーションステートが存在しているため、追加に失敗しました。{addState}");
            return false;
        }

        public bool AddBlend(EasyBlendTree tree)
        {
            var findState = FindBlend(tree.name);
            if (findState == null)
            {
                blends.Add(new EasyAnimationStateBlend(states, tree));
                DebugLog.Normal($"EasyAnimationStateManager.AddBlend : ブレンドアニメーションステートを追加しました。{tree.name}");
                return true;
            }
            
            DebugLog.Warning($"EasyAnimationStateManager.AddBlend : ブレンドアニメーションステートが存在しているため、追加に失敗しました。{tree.name}");
            return false;
        }

        public bool Remove(string removeStateName)
        {
            var findState = Find(removeStateName);
            if (findState != null)
            {
                findState.Destroy();
                states.Remove(findState);
                DebugLog.Normal($"EasyAnimationStateManager.Remove : アニメーションステートを削除しました。{removeStateName}");
                return true;
            }
            
            DebugLog.Warning($"EasyAnimationStateManager.Remove : アニメーションステートが存在していないため、追加に失敗しました。{removeStateName}");
            return false;
        }
        
        public bool RemoveBlend(string removeStateName)
        {
            var findState = FindBlend(removeStateName);
            if (findState != null)
            {
                blends.Remove(findState);
                DebugLog.Normal($"EasyAnimationStateManager.RemoveBlend : ブレンドアニメーションステートを削除しました。{removeStateName}");
                return true;
            }
            
            DebugLog.Warning($"EasyAnimationStateManager.RemoveBlend : ブレンドアニメーションステートが存在していないため、追加に失敗しました。{removeStateName}");
            return false;
        }

        public int Count()
        {
            return states.Count;
        }
    }
}