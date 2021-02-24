using System.Collections.Generic;

namespace Framework
{
    public interface IDataStore<TModel, TPrimaryKey> 
        where TModel : IModel
        where TPrimaryKey : IPrimaryKey<TPrimaryKey, TModel>
    {
        Dictionary<TPrimaryKey, TModel> Load();
    }
}
