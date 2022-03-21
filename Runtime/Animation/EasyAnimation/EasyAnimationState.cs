using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Framework
{
    public class EasyAnimationState
    {
        readonly AnimationClipPlayable _playable;
        readonly string _stateName;
        readonly AnimationClip _clip;
        readonly WrapMode _wrapMode;

        public Playable Playable => _playable;
        public string StateName => _stateName;
        public AnimationClip Clip => _clip;
        public WrapMode WrapMode => _wrapMode;

        public float weight;
        public int index;
        public float destinationWeight;
        public float originWeight;
        public bool isBlending;

        public EasyAnimationState(AnimationClip clip, string stateName, PlayableGraph graph)
        {
            _stateName = stateName;
            _playable = AnimationClipPlayable.Create(graph, clip);
            _playable.SetApplyFootIK(false);
            _playable.SetApplyPlayableIK(false);
            
            if (!clip.isLooping || clip.wrapMode == WrapMode.Once)
            {
                _playable.SetDuration(clip.length);
            }

            _clip = clip;
            _wrapMode = clip.wrapMode;
            destinationWeight = 0f;
            originWeight = 0f;
        }

        public void Play()
        {
            _playable.Play();
            _playable.SetDone(false);
        }

        public void Stop()
        {
            _playable.Pause();
            _playable.SetDone(true);
        }

        public bool IsPlaying()
        {
            return !_playable.IsDone();
        }

        public bool IsPlayingOnce()
        {
            float elapsedTime = GetElapsedTime();
            // -0.01しないと小数点誤差がでることがある
            return elapsedTime <= Clip.length - 0.01f;
        }

        public float GetElapsedTime()
        {
            return (float) _playable.GetTime();
        }

        public float GetTime()
        {
            return GetElapsedTime() % _clip.length;
        }

        public void SetTime(float time)
        {
            // UnityBug : https://forum.unity.com/threads/resetting-animationclipplayable-causes-events-to-play-multiple-times.614047/
            _playable.SetTime(time);
            _playable.SetTime(time);

            if (time >= _playable.GetDuration())
            {
                Stop();
            }
        }

        public float GetSpeed()
        {
            return (float) _playable.GetSpeed();
        }

        public void SetSpeed(float speed)
        {
            _playable.SetSpeed(speed);
        }

        public void Destroy()
        {
            if (_playable.IsValid())
            {
                _playable.GetGraph().DestroySubgraph(_playable);
            }
        }

        public override string ToString()
        {
            return $"{_stateName}:{index}:{weight}:{WrapMode}:{_clip.length}";
        }
    }
}