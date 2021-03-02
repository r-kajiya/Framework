using System.Collections.Generic;

namespace Framework
{
    public interface IPlayerRepository<TModel, in TPrimaryKey>
        where TModel : IModel
        where TPrimaryKey : IPrimaryKey<TPrimaryKey, TModel>
    {
        void Load();
        void Save(TModel model);
        TModel Get(TPrimaryKey primaryKey);
    }
}
