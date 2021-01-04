using UnityEngine;

namespace Framework
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        static T _instance;

        public static T I
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();

                    if (_instance == null)
                    {
                        GameObject go = new GameObject();
                        go.name = typeof(T).Name;
                        _instance = go.AddComponent<T>();
                    }
                }

                return _instance;
            }
        }

        protected void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
                OnAwake();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnAwake(){}
    }	
}
