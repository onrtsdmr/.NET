using System.Collections.Generic;
using shopapp.business.Abstract;
using shopapp.dataaccess.Abstract;

namespace shopapp.business.Concrete
{
    public class GenericManager<TEntity, TRepository>: IService<TEntity>
        where TRepository: IRepository<TEntity>
    {
        public GenericManager()
        {
            
        }
        
        public TEntity GetById(int id)
        {
            return GetById(id);
        }

        public List<TEntity> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public bool Create(TEntity entity)
        {
            throw new System.NotImplementedException();
        }

        public void Update(TEntity entity)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(TEntity entity)
        {
            throw new System.NotImplementedException();
        }
    }
}