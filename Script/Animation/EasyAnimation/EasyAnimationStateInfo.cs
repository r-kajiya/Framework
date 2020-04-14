using UnityEngine;
using UnityEngine.Playables;

namespace Framework
{
    public class EasyAnimationStateInfo
    {
        Playable _playable;
        AnimationClip _clip;
        WrapMode _wrapMode;
        
        public Playable Playable => _playable;
        
        public EasyAnimationStateInfo(string stateName, AnimationClip clip, Playable playable)
        {
            _playable = playable;
            _clip = clip;
            _wrapMode = clip.wrapMode;
            _playable.GetTime();
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
            return (float)_playable.GetTime();
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