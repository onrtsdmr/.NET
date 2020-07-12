using System.Collections.Generic;

namespace shopapp.business.Abstract
{
    public interface IService<TEntity>
    {
        TEntity GetById(int id);
        List<TEntity> GetAll ();
        bool Create(TEntity entity);
        void Update (TEntity entity);
        void Delete (TEntity entity);
    }
}