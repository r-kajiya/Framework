using System;
using System.Collections;
using UnityEngine;

namespace Framework
{
    public class AbsolutelyActiveCoroutine : MonoBehaviour
    {
        static AbsolutelyActiveCoroutine instance;

        static void CreateIfNeeded()
        {
            if (instance == null)
            {
                GameObject obj = new GameObject();
                obj.name = "AbsolutelyActiveCoroutine";
                instance = obj.AddComponent<AbsolutelyActiveCoroutine>();
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

