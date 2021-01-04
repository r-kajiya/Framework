using System;

namespace Framework
{
    public class Subscribe<T> : IObserver<T>
    {
        readonly Action<T> _onNext;
        readonly Action<Exception> _onError;
        readonly Action _onCompleted;

        public Subscribe(Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            _onNext = onNext;
            _onError = onError;
            _onCompleted = onCompleted;
        }

        public void OnNext(T value)
        {
            if (_onNext != null)
            {
                _onNext(value);
            }
        }

        public void OnError(Exception error)
        {
            if (_onError != null)
            {
                _onError(error);
            }
        }

        public void OnCompleted()
        {
            if (_onCompleted != null)
            {
                _onCompleted();
            }
        }
    }
}