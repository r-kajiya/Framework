using System;
using Sirenix.Serialization;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Framework
{
    [Serializable]
    public abstract class AnimationEventObjectExecutor { }
    
    [Serializable]
    public class AnimationEventObjectExecutorPlayEffect : AnimationEventObjectExecutor
    {
        [OdinSerialize]
        GameObject _effectPrefab;

        [OdinSerialize]
        string _effectParentName;

        public void Play(AnimationEventInvoker invoker)
        {
            Transform parent = null;
            var children = invoker.GetComponentsInChildren<Transform>( true );
            foreach ( var child in children )
            {
                if ( child.name == _effectParentName )
                {
                    parent = child;
                }
            }

            if (parent == null)
            {
                parent = invoker.transform;
            }

            if (_effectPrefab == null)
            {
                DebugLog.Error("AnimationEventObjectPlayEffect:Play:エフェクトプレハブが設定されていません", DebugLogColor.animationEvent);
                return;
            }

            if (Application.isPlaying)
            {
                var effectObj = Object.Instantiate(_effectPrefab, parent);
                var system = effectObj.GetComponent<ParticleSystem>();
                system.Play();
            }
#if UNITY_EDITOR
            else
            {
                OnEditorEnter(parent);
            }
#endif
        }
        
#if UNITY_EDITOR
        
        class SimulateParticle
        {
            public ParticleSystem ParticleSystem { get; set; }
            public float Time { get; set; }
            public float PreviousTime { get; set; }
            public float CurrentTime { get; set; }

            public SimulateParticle(ParticleSystem particleSystem)
            {
                ParticleSystem = particleSystem;
                Restart();
            }

            public void Restart()
            {
                Time = 0.0f;
                CurrentTime = (float)EditorApplication.timeSinceStartup;
                PreviousTime = CurrentTime;
            }
        }

        SimulateParticle _simulateParticleSystem;

        void OnEditorEnter(Transform parent)
        {
            if (_simulateParticleSystem == null)
            {
                var effectObj = Object.Instantiate(_effectPrefab, parent);
                var system = effectObj.GetComponent<ParticleSystem>();
                _simulateParticleSystem = new SimulateParticle(system);
                EditorApplication.update += OnEditorUpdate;
            }
            else
            {
                _simulateParticleSystem.Restart();
            }
        }

        void OnEditorUpdate()
        {
            if (_simulateParticleSystem == null || _simulateParticleSystem.ParticleSystem == null)
            {
                return;
            }
            
            if (_simulateParticleSystem.Time >= 1.0f)
            {
                return;
            }

            _simulateParticleSystem.Time += 0.033f;
            _simulateParticleSystem.ParticleSystem.Simulate(_simulateParticleSystem.Time, true, false, true);
        }
#endif
    }
}