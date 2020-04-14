using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

namespace Framework
{
    public abstract class BaseRepository<TModel, TDataStore> : IRepository<TModel>
        where TModel : ModelBase
        where TDataStore : IDataStore<TModel>
    {
        protected Dictionary<int, TModel> map;
        protected List<TModel> list;
        protected TDataStore dataStore;
        protected bool isDirty;

        protected BaseRepository(TDataStore dataStore)
        {
            this.dataStore = dataStore;
            Load();
        }

        public virtual void Load()
        {
            map = dataStore.Load();
            list = new List<TModel>(map.Values);
        }

        public virtual void Save(TModel model)
        {
            isDirty = true;
            dataStore.Save(model);
        }

        public virtual void SaveList(List<TModel> models)
        {
            isDirty = true;
            dataStore.SaveList(models);
        }

        public virtual TModel Get(int id)
        {
            if (isDirty)
            {
                isDirty = false;
                Load();
            }

            if (Contains(id))
            {
                return map[id];
            }

            return null;
        }

        public virtual List<TModel> GetAll()
        {
            if (isDirty)
            {
                isDirty = false;
                Load();
            }

            return list;
        }

        protected bool Contains(int id)
        {
            return map.ContainsKey(id);
        }
    }
}


