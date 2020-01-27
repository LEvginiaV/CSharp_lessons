using System;
using System.Threading.Tasks;

using Market.Api.Client;
using Market.Api.Models.Products;

namespace Market.CustomersAndStaff.FunctionalTests.Helpers.Products
{
    public class ProductHelper : IProductHelper
    {
        public ProductHelper(IMarketApiClient marketApiClient)
        {
            this.marketApiClient = marketApiClient;
        }

        public async Task<Product> CreateProductAsync(string name, decimal? price = null, ProductUnit unit = ProductUnit.Piece)
        {
            return await marketApiClient.Products.Put(ContextHelper.GetCurrentShopId(), Guid.NewGuid(), new Product
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    ProductCategory = ProductCategory.NonAlcoholic,
                    ProductUnit = unit,
                    PricesInfo = new PriceInfo
                        {
                            SellPrice = price,
                        },
                });
        }

        public async Task<Product> CreateServiceAsync(string name, decimal? price = null)
        {
            return await marketApiClient.Products.Put(ContextHelper.GetCurrentShopId(), Guid.NewGuid(), new Product
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    ProductCategory = ProductCategory.Service,
                    ProductUnit = ProductUnit.Piece,
                    PricesInfo = new PriceInfo
                        {
                            SellPrice = price,
                        },
                });
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            return await marketApiClient.Products.Put(ContextHelper.GetCurrentShopId(), product.Id.Value, product);
        }

        public async Task<Product> RemoveAsync(Product product)
        {
            product.IsDeleted = true;
            return await marketApiClient.Products.Put(ContextHelper.GetCurrentShopId(), product.Id.Value, product);
        }

        private readonly IMarketApiClient marketApiClient;
    }
}