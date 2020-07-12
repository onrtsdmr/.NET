using System;
using System.Collections.Generic;
using shopapp.entity;

namespace shopapp.webui.Models
{
    public class PageInfo
    {
        public int TotalItems { get; set; }    
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public string CurrentCategory { get; set; }

        public int TotalPages()
        {
            return (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);
        }

        public PageInfo(int totalItems, int itemsPerPage, int currentPage, string currentCategory)
        {
            this.TotalItems = totalItems;
            this.ItemsPerPage = itemsPerPage;
            this.CurrentPage = currentPage;
            this.CurrentCategory = currentCategory;
        }
    }
    
    public class ProductListViewModel
    {
        public PageInfo PageInfo { get; set; }
        public List<Product> Products { get; set; }
    }
}