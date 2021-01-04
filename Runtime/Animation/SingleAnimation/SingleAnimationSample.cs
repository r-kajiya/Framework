using UnityEngine;

namespace Framework
{
    public class SingleAnimationSample : MonoBehaviour
    {
        [SerializeField]
        SingleAnimation _singleAnimation = null;

        public void RePlay()
        {
            _singleAnimation.RePlay();
        }

        public void Play()
        {
            _singleAnimation.Play();
        }

        public void Pause()
        {
            _singleAnimation.Pause();
        }

        public void Speed2x()
        {
            _singleAnimation.SetSpeed(2);
        }

        public void Speed1x()
        {
            _singleAnimation.SetSpeed(1);
        }

        public void Speed05x()
        {
            _singleAnimation.SetSpeed(0.5f);
        }
    }
}