using System.Collections.Generic;

namespace Framework
{
    public interface IRepository<TModel, in TPrimaryKey>
        where TModel : IModel
        where TPrimaryKey : IPrimaryKey<TPrimaryKey, TModel>
    {
        void Load();
        void Save(TModel model);
        void SaveList(List<TModel> models);
        TModel Get(TPrimaryKey primaryKey);
        List<TModel> GetAll();
    }
}
