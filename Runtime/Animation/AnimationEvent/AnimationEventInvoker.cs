using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    [ExecuteInEditMode]
    public class AnimationEventInvoker : MonoBehaviour
    {
        [SerializeField]
        AnimationEvents _animationEvents;
        
        [SerializeField]
        bool _convertAutomaticallyOnAwake = false;

        public AnimationEvents AnimationEvents => _animationEvents;

        void Awake()
        {
            if (!_convertAutomaticallyOnAwake)
            {
                return;
            }
            
            var easyAnimation = GetComponent<EasyAnimation>();
            if (easyAnimation != null)
            {
                easyAnimation.ImportAnimationEvents(_animationEvents);
            }
        }

        public void PlayEffect(UnityEngine.Object obj)
        {
            // エフェクトキー,再生する場所(GameObject名),オフセット,回転値オフセット,スケールオフセット
            
            // sample
            var eventObj = obj as AnimationEventObject;
            Debug.Log("PlayEffect");
            Transform parent = null;
            var children = GetComponentsInChildren<Transform>( true );
            foreach ( var child in children )
            {
                if ( child.name == eventObj.EffectParentName )
                {
                    parent = child;
                }
            }
            
            var effectObj = GameObject.Instantiate(eventObj.EffectPrefab, parent);
            var system = effectObj.GetComponent<ParticleSystem>();

            if (Application.isPlaying)
            {
                system.Play();
            }
            else
            {
#if UNITY_EDITOR
                _simulateParticleSystems.Add(new SimulateParticle(system));
                var invocationList = EditorApplication.update.GetInvocationList();
                bool added = invocationList.Any (invocation => invocation.Method.Name == "OnEditorUpdate");

                if (!added)
                {
                    EditorApplication.update += OnEditorUpdate;
                }
#endif
            }
        }

        public void PlaySound(UnityEngine.Object obj)
        {
            Debug.Log("PlaySound");
            // 再生する音キー
        }
        
#if UNITY_EDITOR
        
        class SimulateParticle
        {
            public ParticleSystem ParticleSystem { get; private set; }
            public float Time { get; set; }

            public SimulateParticle(ParticleSystem particleSystem)
            {
                Time = 0.0f;
                ParticleSystem = particleSystem;
            }
        }

        List<SimulateParticle> _simulateParticleSystems = new List<SimulateParticle>();
        void OnEditorUpdate()
        {
            List<SimulateParticle> removeList = new List<SimulateParticle>();
            foreach (var simulateParticle in _simulateParticleSystems)
            {
                if (simulateParticle.Time >= 1.0f)
                {
                    removeList.Add(simulateParticle);
                    continue;
                }
                simulateParticle.Time += Time.deltaTime;
                simulateParticle.ParticleSystem.Simulate(simulateParticle.Time);
            }

            foreach (var remove in removeList)
            {
                _simulateParticleSystems.Remove(remove);
                DestroyImmediate(remove.ParticleSystem.gameObject);
            }
            
            // SceneView.RepaintAll();
        }
#endif
        
    }
}