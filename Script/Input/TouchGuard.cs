using UnityEngine.EventSystems;

namespace Framework
{
    public class TouchGuard : MonoSingleton<TouchGuard>
    {
        EventSystem _eventSystem;

        EventSystem EventSystem
        {
            get
            {
                if (_eventSystem == null)
                {
                    _eventSystem = GetComponent<EventSystem>();
                }

                return _eventSystem;
            }
        }

        public void SetActive(bool enable)
        {
            EventSystem.enabled = enable;
        }
    }
}


