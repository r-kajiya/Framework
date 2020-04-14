using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Framework
{
    [RequireComponent(typeof(Animator))]
    public class SingleAnimation : MonoBehaviour
    {
        [SerializeField]
        AnimationClip _animationClip;

        [SerializeField]
        bool _playAutomatically = false;

        PlayableGraph _playableGraph;
        AnimationClipPlayable _animationClipPlayable;
        Animator _animator;
        bool _isInitialized;
        DirectorUpdateMode _updateMode;
        Action _onStop;
        Action _onFinish;
        
        public Animator Animator
        {
            get
            {
                if (_animator == null)
                {
                    _animator = GetComponent<Animator>();
                }

                return _animator;
            }
        }

        public void Initialize(DirectorUpdateMode updateMode = DirectorUpdateMode.GameTime)
        {
            _isInitialized = false;

            if (_playableGraph.IsValid())
            {
                _playableGraph.Destroy();
            }
            
            _updateMode = updateMode;
            
            _playableGraph = PlayableGraph.Create();
            _playableGraph.SetTimeUpdateMode(_updateMode);

            _animationClipPlayable = AnimationClipPlayable.Create(_playableGraph, _animationClip);
            _animationClipPlayable.SetApplyFootIK(false);
            _animationClipPlayable.SetApplyPlayableIK(false);

            var playableOutput = AnimationPlayableOutput.Create(_playableGraph, "SingleAnimation", Animator);
            playableOutput.SetSourcePlayable(_animationClipPlayable);

            
            _onFinish = () => { Debug.LogWarning("Finish");};
            _onStop = () => { Debug.LogWarning("Stop");};
            
            _isInitialized = true;
        }

        void InitializeIfNeeded()
        {
            if (!_isInitialized)
            {
                Initialize();
            }
        }

        void OnEnable()
        {
            if (_playAutomatically)
            {
                InitializeIfNeeded();
                _playableGraph.Play();
                Play();
            }
        }

        void OnDisable()
        {
            if (_playAutomatically)
            {
                Stop();
                _playableGraph.Stop();
            }
        }

        void OnDestroy()
        {
            _isInitialized = false;

            if (_playableGraph.IsValid())
            {
                _playableGraph.Destroy();
            }
        }

        public void AnimationManualUpdate()
        {
            if (!_isInitialized)
            {
                return;
            }

            if (_updateMode != DirectorUpdateMode.Manual)
            {
                return;
            }

            _playableGraph.Evaluate(Time.deltaTime);
        }

        void Update()
        {
            if (IsFinish())
            {
                _onFinish?.Invoke();
                _onFinish = null;
            }
        }

        public void RePlay()
        {
            _animationClipPlayable.SetTime(0);
            _animationClipPlayable.Play();
        }

        public void Play()
        {
            _animationClipPlayable.Play();
        }

        public void Play(float time)
        {
            _animationClipPlayable.SetTime(time);
            _animationClipPlayable.Play();
        }

        public void PlayNormalizeTime(float normalizeTime)
        {
            float time = _animationClip.length * normalizeTime;
            _animationClipPlayable.SetTime(time);
            _animationClipPlayable.Play();
        }

        public void Stop()
        {
            _animationClipPlayable.Pause();
            _onStop?.Invoke();
            _onStop = null;
        }

        public void SetSpeed(float speed)
        {
            _animationClipPlayable.SetSpeed(speed);
        }

        public bool IsFinish()
        {
            return _animationClipPlayable.GetTime() > _animationClip.length;
        }

        public bool IsLoop()
        {
            return _animationClip.isLooping;
        }

        public void SetStopCallback(Action onAction)
        {
            _onStop = onAction;
        }
        
        public void SetFinishCallback(Action onAction)
        {
            _onFinish = onAction;
        }

        public float GetTime()
        {
            return (float)_animationClipPlayable.GetTime();
        }

        public float GetTimeLoop()
        {
            return  (float)_animationClipPlayable.GetTime() % _animationClip.length;
        }
        
        public float GetTimeNormalize()
        {
            return (float)_animationClipPlayable.GetTime() / _animationClip.length;
        }

        public float GetTimeNormalizeLoop()
        {
            return  (float)_animationClipPlayable.GetTime() % _animationClip.length / _animationClip.length;
        }
        
        public void Reset(AnimationClip animationClip, DirectorUpdateMode updateMode = DirectorUpdateMode.GameTime)
        {
            _animationClip = animationClip;
            Initialize(updateMode);
        }

        public override string ToString()
        {
            return string.Format($"Name:{name}:" +
                                 $", ClipName:{_animationClip.name}" +
                                 $", Time:{_animationClipPlayable.GetTime():F2}" +
                                 $", Length:{_animationClip.length:F2}") +
                                 $", GetTimeLoop:{GetTimeLoop():F2}" +
                                 $", GetTimeNormalize:{GetTimeNormalize():F2}" +
                                 $", GetTimeNormalizeLoop:{GetTimeNormalizeLoop():F2}" +
                                 $", Speed:{_animationClipPlayable.GetSpeed():F2}" +
                                 $", WrapMode:{_animationClip.wrapMode}" +
                                 $", Loop:{_animationClip.isLooping}" +
                                 $", FootIK:{_animationClipPlayable.GetApplyFootIK()}" +
                                 $", PlayableIK:{_animationClipPlayable.GetApplyPlayableIK()}";
        }

#if UNITY_EDITOR

        public void Speed2x()
        {
            SetSpeed(2);
        }

        public void Speed1x()
        {
            SetSpeed(1);
        }

        public void Speed05x()
        {
            SetSpeed(0.5f);
        }

#endif
    }
}