using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStore.Models.Models
{
	public class OrderHeader
	{
		public int Id { get; set; }
		public string ApplicationUserId { get; set; }
		[ValidateNever]
		public ApplicationUser ApplicationUser { get; set; }

		public DateTime OrderDate { get; set; }// 
		public DateTime ShippingDate { get; set; }  //معاد الخروج
		public decimal TotalPrice { get; set; }
		public string? OrderStatus { get; set; }
		public string? PaymentStatus { get; set; }
		public DateTime PaymentDate { get; set; }
		public string? TrakingNumber { get; set; }
		public string? Carrier { get; set; }  //مين الي هيطلعه


	    //Data of user
		public string Name { get; set; }
		public string City { get; set; }
		public string Address { get; set; }
        public string PhoneNumber { get; set; }


        //stripe
        public string? SessionId { get; set; }
		public string? PaymentIntentId { get; set; }
	}
}
