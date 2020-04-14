using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Framework
{
    public class EasyAnimationState
    {
        AnimationClipPlayable _playable;
        AnimationClip _clip;
        WrapMode _wrapMode;

        public Playable Playable => _playable;

        public EasyAnimationState(string stateName, AnimationClip clip, PlayableGraph graph)
        {
            _playable = AnimationClipPlayable.Create(graph, clip);
            _playable.SetApplyFootIK(false);
            _playable.SetApplyPlayableIK(false);
            _clip = clip;
            _wrapMode = clip.wrapMode;
        }

        public void Play()
        {
            _playable.Play();
        }

        public void Pause()
        {
            _playable.Pause();
        }

        public void Stop()
        {
            SetTime(0f);
        }

        public bool IsPlaying()
        {
            return !_playable.IsDone();
        }

        public float GetTime()
        {
            return (float) _playable.GetTime();
        }

        public void SetTime(float time)
        {
            _playable.SetTime(time);

            if (time >= _playable.GetDuration())
            {
                _playable.SetDone(true);
            }
        }
    }
}