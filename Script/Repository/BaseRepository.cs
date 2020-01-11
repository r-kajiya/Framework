using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public abstract class BaseRepository<TModel, TEntity, TDataStore> : IRepository<TModel>
        where TModel : IModel
        where TEntity : IEntity
        where TDataStore : IDataStore<TModel>
    {
        Dictionary<int, TModel> _map;
        List<TModel> _list;
        IDataStore<TModel> _dataStore;
        bool _isDirty;

        public BaseRepository(IDataStore<TModel> dataStore)
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

        public void SaveList(List<TModel> models)
        {
            _isDirty = true;
            _dataStore.SaveList(models);
        }

        public virtual TModel Get(int id)
        {
            if (_isDirty)
            {
                _isDirty = false;
                Load();
            }

            return _map[id];
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

        protected bool Contains(int id)
        {
            return _map.ContainsKey(id);
        }
    }
}


