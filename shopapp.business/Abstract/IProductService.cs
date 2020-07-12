using System.Collections.Generic;
using shopapp.entity;

namespace shopapp.business.Abstract
{
    public interface IProductService : IService<Product>, IVatidator<Product>
    {
        Product GetProductDetails(string url);
        List<Product> GetProductByCategory(string name, int pageSize, int page);
        int GetCountByCategory(string category);

        List<Product> GetHomePageProducts();
        List<Product> GetSearchResult(string searchQuery);

        Product GetByIdWithCategory(int id);

        bool Update(Product entity, int[] categoryIds);
    }
}