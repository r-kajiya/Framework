using UnityEngine;

namespace Framework
{
    public struct Destination
    {
        float _destination;
        float _origin;
        float _time;
        float _transitionDuration;

        public void Start(float origin, float destination, float transitionDuration)
        {
            _origin = origin;
            _destination = destination;
            _transitionDuration = transitionDuration;
            _time = 0f;
        }

        public void Update(float dt)
        {
            if (_transitionDuration <= 0f)
            {
                _transitionDuration = 0.001f;
            }

            float oneFrameAdd = dt * (1f / _transitionDuration);
            _time += oneFrameAdd;

            if (_time >= 1f)
            {
                _time = 1f;
            }
        }

        public float Current()
        {
            return Mathf.Lerp(_origin, _destination, _time);
        }
    }

    public struct Destination2D
    {
        Vector2 _destination;
        Vector2 _origin;
        float _time;
        float _transitionDuration;

        public void Start(Vector2 origin, Vector2 destination, float transitionDuration)
        {
            _origin = origin;
            _destination = destination;
            _transitionDuration = transitionDuration;
            _time = 0f;
        }

        public void Update(float dt)
        {
            if (_transitionDuration <= 0f)
            {
                _transitionDuration = 0.001f;
            }

            float oneFrameAdd = dt * (1f / _transitionDuration);
            _time += oneFrameAdd;

            if (_time >= 1f)
            {
                _time = 1f;
            }
        }

        public Vector2 Current()
        {
            return Vector2.Lerp(_origin, _destination, _time);
        }
    }

    public struct Destination3D
    {
        Vector3 _destination;
        Vector3 _origin;
        float _time;
        float _transitionDuration;

        public void Start(Vector3 origin, Vector3 destination, float transitionDuration)
        {
            _origin = origin;
            _destination = destination;
            _transitionDuration = transitionDuration;
            _time = 0f;
        }

        public void Update(float dt)
        {
            if (_transitionDuration <= 0f)
            {
                _transitionDuration = 0.001f;
            }

            float oneFrameAdd = dt * (1f / _transitionDuration);
            _time += oneFrameAdd;

            if (_time >= 1f)
            {
                _time = 1f;
            }
        }

        public Vector3 Current()
        {
            return Vector3.Lerp(_origin, _destination, _time);
        }
    }

    public struct Destination4D
    {
        Vector4 _destination;
        Vector4 _origin;
        float _time;
        float _transitionDuration;

        public void Start(Vector4 origin, Vector4 destination, float transitionDuration)
        {
            _origin = origin;
            _destination = destination;
            _transitionDuration = transitionDuration;
            _time = 0f;
        }

        public void Update(float dt)
        {
            if (_transitionDuration <= 0f)
            {
                _transitionDuration = 0.001f;
            }

            float oneFrameAdd = dt * (1f / _transitionDuration);
            _time += oneFrameAdd;

            if (_time >= 1f)
            {
                _time = 1f;
            }
        }

        public Vector4 Current()
        {
            return Vector4.Lerp(_origin, _destination, _time);
        }
    }
}