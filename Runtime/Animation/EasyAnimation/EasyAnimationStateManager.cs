using System.Collections.Generic;

namespace Framework
{
    public class EasyAnimationStateManager
    {
        List<EasyAnimationState> _states = new List<EasyAnimationState>();

        public List<EasyAnimationState> States => _states;

        public EasyAnimationStateCrossFade crossFadeTarget; 

        public EasyAnimationState Find(string stateName)
        {
            return _states.Find(x => x.StateName == stateName);
        }
        
        public EasyAnimationState Find(int stateIndex)
        {
            return _states.Find(x => x.index == stateIndex);
        }
        
        public bool Exists(string stateName)
        {
            return _states.Exists(x => x.StateName == stateName);
        }
        
        public bool Add(EasyAnimationState addState)
        {
            var findState = Find(addState.StateName);
            if (findState == null)
            {
                _states.Add(addState);
                DebugLog.Normal($"EasyAnimationStateManager.Add : アニメーションステートを追加しました。{addState}");
                return true;
            }
            
            DebugLog.Warning($"EasyAnimationStateManager.Add : アニメーションステートが存在しているため、追加に失敗しました。{addState}");
            return false;
        }
        
        public bool Remove(EasyAnimationState removeState)
        {
            return Remove(removeState.StateName);
        }

        public bool Remove(string removeState)
        {
            var findState = Find(removeState);
            if (findState != null)
            {
                findState.Destroy();
                _states.Remove(findState);
                DebugLog.Normal($"EasyAnimationStateManager.Remove : アニメーションステートを削除しました。{removeState}");
                return true;
            }
            
            DebugLog.Warning($"EasyAnimationStateManager.Remove : アニメーションステートが存在していないため、追加に失敗しました。{removeState}");
            return false;
        }

        public int Count()
        {
            return _states.Count;
        }
    }
}