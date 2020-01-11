using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class SystemContext : Context
    {
        [SerializeField]
        RootPresenter _rootPresenter = null;

        static Dictionary<string, SystemContext> _systemContexts = new Dictionary<string, SystemContext>();

        static protected Dictionary<string, SystemContext> SystemContexts
        {
            get { return _systemContexts; }
        }
            
        public override void Run()
        {
            if (!_systemContexts.ContainsKey(gameObject.name))
            {
                _systemContexts.Add(gameObject.name, this);
            }
            
            _rootPresenter.Disable();
        }

        public IEnumerator DoInitRun()
        {
            _rootPresenter.Enable();
            yield return DoPreLoad(null);
            yield return DoLoad(null);
            yield return DoLoaded(null);
        }

        protected virtual IEnumerator DoPreLoad(SystemContextContainer container) { yield break; }
        protected virtual IEnumerator DoLoad(SystemContextContainer container) { yield break; }
        protected virtual IEnumerator DoLoaded(SystemContextContainer container) { yield break; }

        protected virtual IEnumerator DoPreUnload() { yield break; }
        protected virtual IEnumerator DoUnload() { yield break; }
        protected virtual IEnumerator DoUnloaded() { yield break; }

        protected void ChangeContext(
            SystemContext next,
            SystemContextContainer container = null,
            System.Func<IEnumerator> onCurPreUnload = null,
            System.Func<IEnumerator> onCurUnload = null,
            System.Func<IEnumerator> onCurUnloaded = null,
            System.Func<IEnumerator> onNextPreLoad = null,
            System.Func<IEnumerator> onNextLoad = null,
            System.Func<IEnumerator> onNextLoaded = null)
        {
            SystemContextChanger changer = new SystemContextChanger();

            changer.OnCurPreUnload = onCurPreUnload;
            changer.OnCurUnload = onCurUnload;
            changer.OnCurUnloaded = onCurUnloaded;
            changer.OnNextPreLoad = onNextPreLoad;
            changer.OnNextLoad = onNextLoad;
            changer.OnNextLoaded = onNextLoaded;

            changer.Execute(this, next, container);
        }

        protected class SystemContextChanger
        {
            public System.Func<IEnumerator> OnCurPreUnload;
            public System.Func<IEnumerator> OnCurUnload;
            public System.Func<IEnumerator> OnCurUnloaded;

            public System.Func<IEnumerator> OnNextPreLoad;
            public System.Func<IEnumerator> OnNextLoad;
            public System.Func<IEnumerator> OnNextLoaded;

            public void Execute(SystemContext self, SystemContext next, SystemContextContainer container)
            {
                AbsolutelyActiveCorutine.Subscribe(DoExecute(self, next, container));
            }

            IEnumerator DoExecute(SystemContext self, SystemContext next, SystemContextContainer container)
            {
                yield return OnCurPreUnload?.Invoke();
                yield return self.DoPreUnload();

                yield return OnCurUnload?.Invoke();
                yield return self.DoUnload();

                yield return OnCurUnloaded?.Invoke();
                yield return self.DoUnloaded();

                self._rootPresenter.Disable();
                next._rootPresenter.Enable();

                yield return OnNextPreLoad?.Invoke();
                yield return next.DoPreLoad(container);

                yield return OnNextLoad?.Invoke();
                yield return next.DoLoad(container);

                yield return OnNextLoaded?.Invoke();
                yield return next.DoLoaded(container);
            }
        }
    }
}


