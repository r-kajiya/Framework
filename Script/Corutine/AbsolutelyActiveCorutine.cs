using System;
using System.Collections;
using UnityEngine;

namespace Framework
{
    public class AbsolutelyActiveCorutine : MonoBehaviour
    {
        static AbsolutelyActiveCorutine instance;

        static void CreateIfNeeded()
        {
            if (instance == null)
            {
                GameObject obj = new GameObject();
                obj.name = "AbsolutelyActiveCorutine";
                instance = obj.AddComponent<AbsolutelyActiveCorutine>();
                DontDestroyOnLoad(obj);
            }
        }

        public static Coroutine Subscribe(IEnumerator routine)
        {
            CreateIfNeeded();

            return instance.StartCoroutine(instance.routine(routine));
        }
        
        public static void Stop(Coroutine coroutine)
        {
            CreateIfNeeded();
            
            instance.StopCoroutine(coroutine);
        }

        public static Coroutine WaitSecondInvoke(Action onAction,float second)
        {
            CreateIfNeeded();
            
            return instance.StartCoroutine(instance.routineWaitSecond(onAction, second));
        }

        IEnumerator routine(IEnumerator src)
        {
            yield return StartCoroutine(src);
        }
        
        IEnumerator routineWaitSecond(Action onAction, float second)
        {
            yield return new WaitForSeconds(second);
            onAction.Invoke();
        }
    }
}

