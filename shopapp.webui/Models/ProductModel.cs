using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using shopapp.entity;

namespace shopapp.webui.Models
{
    public class ProductModel
    {
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        [Display(Name = "Product Name")]
        [StringLength(50,MinimumLength = 5, ErrorMessage = "Name is the range of 5-50 characters.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Price is required.")]
        [Range(1,10000,ErrorMessage = "Price is the range 1-10000.")]
        public double? Price { get; set; }
        [Required(ErrorMessage = "Description is required.")]
        [StringLength(1000,MinimumLength = 5, ErrorMessage = "Description is the range of 5-100 characters.")]
        public string Description { get; set; }
        [Required(ErrorMessage = "ImageUrl is required.")]
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public bool IsApproved { get; set; }
        public bool IsHome { get; set; }
        [Required(ErrorMessage = "Url is required.")]
        public string Url { get; set; }

        public List<Category> SelectedCategories { get; set; }
    }
}