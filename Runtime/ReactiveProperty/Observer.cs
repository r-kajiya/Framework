using UnityEngine;
using System;

namespace Framework
{
    public static class Observer
    {
        public static IObserver<T> CreateObserver<T>(Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            return new Subscribe<T>(onNext, onError, onCompleted);
        }
    }
}