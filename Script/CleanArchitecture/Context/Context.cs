using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public abstract class Context : MonoBehaviour
    {
        [SerializeField]
        List<Context> _children = null;

        public virtual void Run()
        {
            RunChildren();
        }

        protected void RunChildren()
        {
            foreach (var child in _children)
            {
                child.Run();
            }
        }
    }
}


