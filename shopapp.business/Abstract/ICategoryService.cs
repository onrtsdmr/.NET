using shopapp.entity;

namespace shopapp.business.Abstract
{
    public interface ICategoryService: IService<Category>, IVatidator<Category>
    {
        Category GetByIdWithProduct(int id);
        
        void DeleteFromCategory(int productId, int categoryId);
    }
}