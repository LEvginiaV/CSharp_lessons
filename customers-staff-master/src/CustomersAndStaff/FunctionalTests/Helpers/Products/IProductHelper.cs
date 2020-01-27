using System;
using System.Threading.Tasks;

using Market.Api.Models.Products;

namespace Market.CustomersAndStaff.FunctionalTests.Helpers.Products
{
    public interface IProductHelper
    {
        Task<Product> CreateProductAsync(string name, decimal? price = null, ProductUnit unit = ProductUnit.Piece);
        Task<Product> CreateServiceAsync(string name, decimal? price = null);
        Task<Product> UpdateAsync(Product product);
        Task<Product> RemoveAsync(Product product);
    }
}