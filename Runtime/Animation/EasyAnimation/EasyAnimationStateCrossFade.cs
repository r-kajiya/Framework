namespace Framework
{
    public class EasyAnimationStateCrossFade
    {
        readonly EasyAnimationState _state;
        readonly float _normalizedTransitionDuration;

        public EasyAnimationState State => _state;

        public EasyAnimationStateCrossFade(EasyAnimationState state, float normalizedTransitionDuration)
        {
            _state = state;
            _normalizedTransitionDuration = normalizedTransitionDuration;
        }

        public float UpdateWeight(float dt)
        {
            float otherWeight = 0f;
            
            if (MathHelper.EqualsZero(_normalizedTransitionDuration))
            {
                _state.weight = 1f;
                return otherWeight;
            }
            
            float oneFrameAddWeight = dt * (1f / _normalizedTransitionDuration);
            _state.weight += oneFrameAddWeight;
            otherWeight = 1 - _state.weight;

            if (_state.weight >= 1f)
            {
                _state.weight = 1f;
                otherWeight = 0f;
            }

            return otherWeight;
        }

        public bool IsFinish()
        {
            return _state.weight >= 1f;
        }
    }
}