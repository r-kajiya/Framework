using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class RootPresenter : MonoBehaviour
    {
        public void Disable()
        {
            gameObject.SetActive(false);
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }
    }
}


