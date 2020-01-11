using UnityEngine;

namespace Framework
{
    public class Environment : MonoBehaviour
    {
        void Awake()
        {
            Application.targetFrameRate = 60;
        }
    }
}
