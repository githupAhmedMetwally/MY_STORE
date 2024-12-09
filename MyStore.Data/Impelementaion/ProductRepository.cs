using Microsoft.EntityFrameworkCore;
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
        public List<Product> Search(string temp)
        {
            //To Join Brand Table    To Search by name of product        or name of category
            var result = context.Products.Include(e => e.Category).Where(e => e.Name.Contains(temp)||e.Category.Name.Contains(temp)).ToList();
            return result;

        }

        public List<Product> GetAllWithOrderByAsc()
        {
            var result = context.Products.OrderBy(e => e.Price).ToList();
            return result;
        }
        public List<Product> GetAllWithOrderByDesc()
        {
            var result = context.Products.OrderByDescending(e => e.Price).ToList();
            return result;
        }
    }
}
