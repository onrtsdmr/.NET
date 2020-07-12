using System.Collections.Generic;
using shopapp.entity;

namespace shopapp.dataaccess.Abstract {
    public interface IProductRepository: IRepository<Product>
    {
        List<Product> GetSearchResult(string searchQuery);

        Product GetProductDetails(string url);
        List<Product> GetProductsByCategory(string name, int pageSize, int page);
        List<Product> GetHomePageProducts();
        int GetCountByCategory(string category);

        Product GetByIdWithCategory(int id);

        void Update(Product entity, int[] categoryIds);

    }
}
