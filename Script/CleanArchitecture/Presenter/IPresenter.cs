namespace Framework
{
    public interface IPresenter<out TView>
        where TView : IView
    {
        TView View { get; }
    }
}