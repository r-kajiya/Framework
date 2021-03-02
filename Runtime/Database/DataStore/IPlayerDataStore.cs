using System.Collections.Generic;

namespace Framework
{
    public interface IPlayerDataStore<TModel, TPrimaryKey>
        where TModel : IModel
        where TPrimaryKey : IPrimaryKey<TPrimaryKey, TModel>
    {
        Dictionary<TPrimaryKey, TModel> Load();
        void Save(TModel model);
    }
}
