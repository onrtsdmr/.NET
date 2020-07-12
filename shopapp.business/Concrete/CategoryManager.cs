using System.Collections.Generic;
using shopapp.business.Abstract;
using shopapp.dataaccess.Abstract;
using shopapp.entity;

namespace shopapp.business.Concrete
{
    public class CategoryManager : ICategoryService
    {
        private ICategoryRepository _categoryRepository;

        public CategoryManager(ICategoryRepository categoryRepository)
        {
            this._categoryRepository = categoryRepository;
        }

        public Category GetById(int id)
        {
            return _categoryRepository.GetById(id);
        }

        public List<Category> GetAll()
        {
            return _categoryRepository.GetAll();
        }

        public bool Create(Category entity)
        {
            if (Validation(entity))
            {
                _categoryRepository.Create(entity);
                return true;
            }

            return false;
        }

        public void Update(Category entity)
        {
            _categoryRepository.Update(entity);
        }

        public void Delete(Category entity)
        {
            _categoryRepository.Delete(entity);
        }

        public Category GetByIdWithProduct(int id)
        {
            return _categoryRepository.GetByIdWithProduct(id);
        }

        public void DeleteFromCategory(int productId, int categoryId)
        {
            _categoryRepository.DeleteFromCategory(productId, categoryId);
        }

        public string ErrorMessage { get; set; }

        public bool Validation(Category entity)
        {
            var isValid = true;

            if (string.IsNullOrEmpty(entity.Name))
            {
                ErrorMessage += "Category name is required.\n";
                isValid = false;
            }
            if (string.IsNullOrEmpty(entity.Url))
            {
                ErrorMessage += "Category url is required.\n";
                isValid = false;
            }

            return isValid;
        }
    }
}