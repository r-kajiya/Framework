using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class MainContext : Context
    {
        [SerializeField]
        SystemContext _firstSystemContext = null;

        void Awake()
        {
            Run();
            
            AbsolutelyActiveCorutine.Subscribe(_firstSystemContext.DoInitRun());
        }
    }
}

