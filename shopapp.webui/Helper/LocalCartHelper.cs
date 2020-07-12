using System.Collections.Generic;
using shopapp.entity;

namespace shopapp.webui.Helper
{
    public static class LocalCartHelper
    {
        public static List<Product> CartList = new List<Product>();
        public static void AddCart(Product p) => CartList.Add(p);
        public static void RemoveCart(int id) => CartList.RemoveAt(CartList.FindIndex(p => p.ProductId == id));
        public static void ClearCart() => CartList.Clear();
    }
}