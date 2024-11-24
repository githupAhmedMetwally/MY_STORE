using MyStore.Models.Models;
using MyStore.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStore.Data.Impelementaion
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext context;

        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public void Update(Product product)
        {
            var productId=context.Products.FirstOrDefault(x=>x.Id==product.Id);
            if (productId != null) { 
                productId.Name=product.Name;
                productId.Description=product.Description;
                productId.Price=product.Price;
                productId.Img=product.Img;
                productId.CategoryId=product.CategoryId;
            }
        }
    }
}
