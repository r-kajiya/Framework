using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class AbsolutelyActiveCorutine : MonoBehaviour
    {
        static AbsolutelyActiveCorutine instance;

        public static Coroutine Subscribe(IEnumerator routine)
        {
            if (instance == null)
            {
                GameObject obj = new GameObject();
                obj.name = "AbsolutelyActiveCorutine";
                instance = obj.AddComponent<AbsolutelyActiveCorutine>();
                DontDestroyOnLoad(obj);
            }

            return instance.StartCoroutine(instance.routine(routine));
        }

        IEnumerator routine(IEnumerator src)
        {
            yield return StartCoroutine(src);
        }
    }
}

