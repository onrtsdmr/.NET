using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using shopapp.dataaccess.Abstract;
using shopapp.entity;

namespace shopapp.dataaccess.Concrete.EfCore
{
    public class EfCoreCategoryRepository : EfCoreGenericRepository<Category, ShopContext>, ICategoryRepository
    {
        public List<Category> GetPopularCategories()
        {
            using (var context = new ShopContext())
            {
                return context.Categories.ToList();
            }
        }

        public Category GetByIdWithProduct(int id)
        {
            using (var context = new ShopContext())
            {
                return context.Categories
                    .Where(i=>i.CategoryId == id)
                    .Include(i=>i.ProductCategories)
                    .ThenInclude(i=>i.Product)
                    .FirstOrDefault();
            }
        }

        public void DeleteFromCategory(int productId, int categoryId)
        {
            using (var context = new ShopContext())
            {
                var query = "DELETE FROM productcategory WHERE ProductId=@p0 AND CategoryID=@p1";
                context.Database.ExecuteSqlRaw(query, productId, categoryId);
            }
        }
    }
}