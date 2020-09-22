namespace Framework
{
    public interface IUseCase
    {
        void OnUpdate(float dt);
    }
    
    public class UseCaseBase<TContainer, TPresenter, TView> : IUseCase
        where TView : IView
        where TPresenter : IPresenter<TView>
        where TContainer : IContainer<TPresenter, TView>
    {
        readonly TContainer _container;
        protected TContainer Container => _container;

        protected UseCaseBase(TContainer container)
        {
            _container = container;
        }

        public virtual void OnUpdate(float dt) { }
    }
}

