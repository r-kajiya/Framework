using System.Collections.Generic;

namespace Framework
{
    public class BaseRepository<TModel, TDataStore, TPrimaryKey> : IRepository<TModel, TPrimaryKey>
        where TModel : ModelBase
        where TDataStore : IDataStore<TModel, TPrimaryKey>
        where TPrimaryKey : IPrimaryKey<TPrimaryKey, TModel>
    {
        Dictionary<TPrimaryKey, TModel> _map;
        List<TModel> _list;
        protected TDataStore _dataStore;
        bool _isDirty;

        public BaseRepository(TDataStore dataStore)
        {
            _dataStore = dataStore;
            Load();
        }

        public void Load()
        {
            _map = _dataStore.Load();
            _list = new List<TModel>(_map.Values);
        }

        public TModel Get(TPrimaryKey primaryKey)
        {
            if (_isDirty)
            {
                _isDirty = false;
                Load();
            }

            if (Contains(primaryKey))
            {
                return _map[primaryKey];
            }

            return null;
        }

        public List<TModel> GetAll()
        {
            if (_isDirty)
            {
                _isDirty = false;
                Load();
            }

            return _list;
        }

        bool Contains(TPrimaryKey primaryKey)
        {
            return _map.ContainsKey(primaryKey);
        }
    }
}


