using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class ApplicationContext : Context
    {
        [SerializeField]
        SystemContext _firstSystemContext = null;

        void Awake()
        {
            // ゲーム起動時のSystemContextをすべてオフに
            Run();

            // 初回起動シーンをロードする
            AbsolutelyActiveCorutine.Subscribe(_firstSystemContext.DoInitRun());
        }
    }
}

