namespace Framework
{
    public interface IModel
    {
        int ID { get; }
    }

    public class ModelBase : IModel
    {
        public int ID { get; }
    }
}
