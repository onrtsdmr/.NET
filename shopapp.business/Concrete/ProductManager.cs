using System.Collections.Generic;
using shopapp.business.Abstract;
using shopapp.dataaccess.Abstract;
using shopapp.entity;

namespace shopapp.business.Concrete
{
    public class ProductManager : IProductService
    {
        private IProductRepository _productRepository;

        public ProductManager(IProductRepository productRepository)
        {
            this._productRepository = productRepository;
        }

        public bool Create(Product entity)
        {
            if (Validation(entity))
            {
                _productRepository.Create(entity);
                return true;
            }

            return false;
        }

        public void Delete(Product entity)
        {
            _productRepository.Delete(entity);
        }

        public Product GetProductDetails(string name)
        {
            return _productRepository.GetProductDetails(name);
        }

        public List<Product> GetProductByCategory(string name, int pageSize, int page)
        {
            if (page <= 0)
            {
                page = 1;
            }

            return _productRepository.GetProductsByCategory(name, pageSize, page);
        }

        public int GetCountByCategory(string category)
        {
            return _productRepository.GetCountByCategory(category);
        }

        public List<Product> GetHomePageProducts()
        {
            return _productRepository.GetHomePageProducts();
        }

        public List<Product> GetSearchResult(string searchQuery)
        {
            return _productRepository.GetSearchResult(searchQuery);
        }

        public Product GetByIdWithCategory(int id)
        {
            return _productRepository.GetByIdWithCategory(id);
        }


        public List<Product> GetAll()
        {
            return _productRepository.GetAll();
        }

        public Product GetById(int id)
        {
            return _productRepository.GetById(id);
        }

        public void Update(Product entity)
        {
            _productRepository.Update(entity);
        }

        public bool Update(Product entity, int[] categoryIds)
        {
            if (Validation(entity))
            {
                if (categoryIds.Length!=0)
                {
                    _productRepository.Update(entity, categoryIds);
                    return true;
                }
                else
                {
                    ErrorMessage += "You must choose a category for the product.";
                    return false;
                }
                return true;
            }

            return false;
        }

        public string ErrorMessage { get; set; }

        public bool Validation(Product entity)
        {
            var isValid = true;

            if (string.IsNullOrEmpty(entity.Name))
            {
                ErrorMessage += "Product name is required.\n";
                isValid = false;
            }
            else if (entity.Price < 0)
            {
                ErrorMessage += "Product price cannot be negative.\n";
                isValid = false;
            }

            return isValid;
        }
    }
}
/*
        


 */