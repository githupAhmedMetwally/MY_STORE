﻿using MyStore.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStore.Models.Repositories
{
    public interface IProductRepository:IGenericRepository<Product>
    {
        void Update(Product product);
    }
}