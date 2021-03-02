using System.Collections.Generic;
using System.Linq;

namespace Framework
{
    public class PlayerBaseRepository<TModel, TDataStore, TPrimaryKey> : IPlayerRepository<TModel, TPrimaryKey>
        where TModel : ModelBase
        where TDataStore : IPlayerDataStore<TModel, TPrimaryKey>
        where TPrimaryKey : IPrimaryKey<TPrimaryKey, TModel>
    {
        Dictionary<TPrimaryKey, TModel> _map;
        List<TModel> _list;
        protected TDataStore _dataStore;
        bool _isDirty;

        public PlayerBaseRepository(TDataStore dataStore)
        {
            _dataStore = dataStore;
            Load();
        }

        public void Load()
        {
            _map = _dataStore.Load();
            _list = new List<TModel>(_map.Values);
        }

        public void Save(TModel model)
        {
            _isDirty = true;
            _dataStore.Save(model);
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
        
        public TModel GetOwner()
        {
            if (_isDirty)
            {
                _isDirty = false;
                Load();
            }

            if (_map.Count == 0)
            {
                return null;
            }

            return _map.First().Value;
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


