namespace Framework
{
    public interface IContainer<out TPresenter, TView>
        where TView : IView
        where TPresenter : IPresenter<TView>
    {
        TPresenter Presenter { get; }
    }
}