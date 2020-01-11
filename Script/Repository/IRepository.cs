using System.Collections.Generic;

namespace Framework
{
    public interface IRepository<TModel> where TModel : IModel
    {
        void Load();
        void Save(TModel model);
        void SaveList(List<TModel> models);
        TModel Get(int id);
        List<TModel> GetAll();
    }
}
