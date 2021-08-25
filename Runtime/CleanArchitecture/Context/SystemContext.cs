using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class SystemContext : Context
    {
        static readonly Dictionary<string, SystemContext> systemContexts = new Dictionary<string, SystemContext>();
        protected static Dictionary<string, SystemContext> SystemContexts => systemContexts;

        protected Dictionary<int, IUseCase> UseCases { get; } = new Dictionary<int, IUseCase>();

        bool _isCurrent;
        
        public override void Run()
        {
            if (!systemContexts.ContainsKey(gameObject.name))
            {
                systemContexts.Add(gameObject.name, this);
            }
        }

        public IEnumerator DoInitRun()
        {
            yield return DoPreLoad(null);
            yield return DoLoad(null);
            yield return DoLoaded(null);
        }

        protected virtual void OnUpdate(float dt)
        {
            foreach (var useCase in UseCases)
            {
                useCase.Value.OnUpdate(dt);
            }
        }
        
        void Update()
        {
            if (!_isCurrent)
            {
                return;
            }
            
            float dt = Time.deltaTime;

            OnUpdate(dt);
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

        class SystemContextChanger
        {
            public System.Func<IEnumerator> OnCurPreUnload;
            public System.Func<IEnumerator> OnCurUnload;
            public System.Func<IEnumerator> OnCurUnloaded;

            public System.Func<IEnumerator> OnNextPreLoad;
            public System.Func<IEnumerator> OnNextLoad;
            public System.Func<IEnumerator> OnNextLoaded;

            public void Execute(SystemContext self, SystemContext next, SystemContextContainer container)
            {
                AbsolutelyActiveCoroutine.Subscribe(DoExecute(self, next, container));
            }

            IEnumerator DoExecute(SystemContext self, SystemContext next, SystemContextContainer container)
            {
                self._isCurrent = false;
                
                yield return OnCurPreUnload?.Invoke();
                yield return self.DoPreUnload();

                yield return OnCurUnload?.Invoke();
                yield return self.DoUnload();

                yield return OnCurUnloaded?.Invoke();
                yield return self.DoUnloaded();

                yield return OnNextPreLoad?.Invoke();
                yield return next.DoPreLoad(container);

                yield return OnNextLoad?.Invoke();
                yield return next.DoLoad(container);

                yield return OnNextLoaded?.Invoke();
                yield return next.DoLoaded(container);

                next._isCurrent = true;
            }
        }
    }
}


