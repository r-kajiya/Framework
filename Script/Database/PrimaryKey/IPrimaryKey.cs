using System;

namespace Framework
{
    public interface IPrimaryKey<TPrimaryKey, in TModel> : IEquatable<TPrimaryKey>
        where TModel : IModel
        where TPrimaryKey : IPrimaryKey<TPrimaryKey, TModel>
    {
        void Setup(TModel model);
    }
}