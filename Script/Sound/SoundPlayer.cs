using UnityEngine;
using System.Collections.Generic;

namespace Framework
{
    public class SoundPlayer : MonoBehaviour
    {
        [SerializeField]
        int _playLimit = 1;

        Dictionary<int, int> _playingLimitMapByID = new Dictionary<int, int>();

        public class PlayInfo
        {
            public int id = -1;
            public AudioSource source;

            public PlayInfo(int playID, AudioSource playSource)
            {
                id = playID;
                source = playSource;
            }
        }

        List<PlayInfo> _playList = new List<PlayInfo>();

        const int ORIGINAL = 0;

        void Awake()
        {
            _playList.Add(new PlayInfo(-1, transform.GetComponent<AudioSource>()));
            if (_playList[ORIGINAL].source == null)
            {
                Debug.LogErrorFormat("{0}のオリジナルaudioSourceが存在しません", name);
            }
        }

        void LateUpdate()
        {
            if (_playList[ORIGINAL].source.loop)
            {
                return;
            }

            foreach (var player in _playList)
            {
                if (player.id == -1)
                {
                    continue;
                }

                if (!player.source.isPlaying)
                {
                    if (_playingLimitMapByID[player.id] > 0)
                    {
                        _playingLimitMapByID[player.id]--;
                    }
                }
            }
        }

        public void Play(int playID)
        {
            bool isSuccess = true;

            if (!_playingLimitMapByID.ContainsKey(playID))
            {
                _playingLimitMapByID.Add(playID, 0);
            }

            foreach (var player in _playList)
            {
                if (player.id == playID)
                {
                    if (_playLimit <= _playingLimitMapByID[playID])
                    {
                        return;
                    }

                    if (player.source.isPlaying)
                    {
                        isSuccess = false;
                    }
                    else
                    {
                        isSuccess = true;
                    }
                }

                if (player.id == -1)
                {
                    player.id = playID;
                }

                if (isSuccess)
                {
                    player.source.Play();
                    _playingLimitMapByID[playID]++;
                    return;
                }
            }

            if (!isSuccess)
            {
                var source = gameObject.AddComponent<AudioSource>();
                source.clip = _playList[ORIGINAL].source.clip;
                source.volume = _playList[ORIGINAL].source.volume;
                source.pitch = _playList[ORIGINAL].source.pitch;
                source.priority = _playList[ORIGINAL].source.priority;
                source.playOnAwake = _playList[ORIGINAL].source.playOnAwake;
                source.loop = _playList[ORIGINAL].source.loop;
                source.Play();
                _playingLimitMapByID[playID]++;

                var player = new PlayInfo(playID, source);

                _playList.Add(player);
            }
        }

        public void Stop()
        {
            foreach (var player in _playList)
            {
                if (player.source.isPlaying)
                {
                    player.source.Stop();
                }
            }

            _playingLimitMapByID.Clear();
        }
    }
}