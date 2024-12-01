using MyStore.Models.Models;
using MyStore.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStore.Data.Impelementaion
{
	public class OrderDetailsRepository : GenericRepository<OrderDetails>, IOrderDetailsRepository
	{
		private readonly ApplicationDbContext context;

		public OrderDetailsRepository(ApplicationDbContext context) : base(context)
		{
			this.context = context;
		}

		public void Update(OrderDetails orderDetails)
		{
			context.OrderDetails.Update(orderDetails);
		}
	}
}
