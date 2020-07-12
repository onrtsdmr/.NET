using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using shopapp.entity;

namespace shopapp.webui.Models
{
    public class CategoryModel
    {
        public int CategoryId { get; set; }
        
        [Display(Name = "Name",Prompt = "Name")]
        public string Name { get; set; }
        [Display(Prompt = "Url")]
        public string Url { get; set; }

        public List<Product> Products { get; set; }
    }
}