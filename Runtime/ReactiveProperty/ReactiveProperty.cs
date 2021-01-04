using System;
using System.Collections.Generic;

namespace Framework
{
	public class ReactiveProperty<T> : IObservable<T>
    {
        static readonly IEqualityComparer<T> _defaultEqualityComparer = CustomEqualityComparer.Get<T>();

        IEqualityComparer<T> _equalityComparer;

        IObserver<T> _observer;

        protected virtual IEqualityComparer<T> EqualityComparer
        {
            get
            {
                return _equalityComparer;
            }
        }

        T _value = default(T);

        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (!EqualityComparer.Equals(_value, value))
                {
                    _value = value;
                    _observer.OnNext(_value);
                }
            }
        }

        public ReactiveProperty() : this(default(T)) { }

        public ReactiveProperty(T value) : this(value, _defaultEqualityComparer) { }

        public ReactiveProperty(T value, IEqualityComparer<T> equalityComparer)
        {
            _value = value;
            _equalityComparer = equalityComparer;
        }
        
        public IDisposable Subscribe(IObserver<T> observer)
        {
            _observer = observer;
            return new Unsubscriber(observer);
        }

        class Unsubscriber : IDisposable
        {
            IObserver<T> _observer;

            public Unsubscriber(IObserver<T> observer)
            {
                _observer = observer;
            }

            public void Dispose()
            {
                _observer.OnCompleted();
                GC.SuppressFinalize(this);
            }
        }
    }

    public static class ReactivePropertyExtensions
    {
        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext)
        {
            return source.Subscribe(Observer.CreateObserver(onNext, null, null));
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError)
        {
            return source.Subscribe(Observer.CreateObserver(onNext, onError, null));
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action onCompleted)
        {
            return source.Subscribe(Observer.CreateObserver(onNext, null, onCompleted));
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            return source.Subscribe(Observer.CreateObserver(onNext, onError, onCompleted));
        }
    }
}