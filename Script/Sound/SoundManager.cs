using UnityEngine;

namespace Framework
{
	public class SoundManager : Singleton<SoundManager>
	{
        SoundPlayer[] _playerBGM;
        SoundPlayer[] _playerSE;

        protected override void OnAwake()
        {
            string path = "Sound/SE/Prefab/";
            var playerSE = Resources.LoadAll<SoundPlayer>(path);
            _playerSE = new SoundPlayer[playerSE.Length];

            for(int i = 0; i < playerSE.Length; i++)
            {
                _playerSE[i] = Instantiate<SoundPlayer>(playerSE[i], SoundManager.I.transform);
                _playerSE[i].name = _playerSE[i].name.Replace("(Clone)", "");
            }

            path = "Sound/BGM/Prefab/";
            var playerBGM = Resources.LoadAll<SoundPlayer>(path);
            _playerBGM = new SoundPlayer[playerBGM.Length];

            for (int i = 0; i < playerBGM.Length; i++)
            {
                _playerBGM[i] = Instantiate<SoundPlayer>(playerBGM[i], SoundManager.I.transform);
                _playerBGM[i].name = _playerBGM[i].name.Replace("(Clone)", "");
            }
        }

        public void PlaySE(string audioFileName, int playID)
        {
            foreach (var player in _playerSE)
            {
                if (player.name == audioFileName)
                {
                    player.Play(playID);
                }
            }
        }

        public void PlayBGM(string audioFileName, int playID)
        {
            foreach (var player in _playerBGM)
            {
                if (player.name == audioFileName)
                {
                    player.Play(playID);
                }
                else
                {
                    player.Stop();
                }
            }
        }
	}	
}