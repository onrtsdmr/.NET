using System.Collections.Generic;
using shopapp.entity;

namespace shopapp.dataaccess.Abstract
{
    public interface ICategoryRepository: IRepository<Category>
    {
        List<Category> GetPopularCategories();

        Category GetByIdWithProduct(int id);
        
        void DeleteFromCategory(int productId, int categoryId);
    }
}