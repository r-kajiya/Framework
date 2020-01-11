using System.Collections.Generic;

namespace Framework
{
    public interface IDataStore<TModel> where TModel : IModel
    {
        Dictionary<int, TModel> Load();
        void Save(TModel model);
        void SaveList(List<TModel> models);
    }
}
