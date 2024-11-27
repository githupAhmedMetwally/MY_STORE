using MyStore.Models.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStore.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public Product Product { get; set; }
        [Range(1,100,ErrorMessage ="you must enter number between 1 and 100")]
        public int Count { get; set; }
    }
}
