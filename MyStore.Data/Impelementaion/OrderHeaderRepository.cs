using MyStore.Models.Models;
using MyStore.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStore.Data.Impelementaion
{
	public class OrderHeaderRepository : GenericRepository<OrderHeader>, IOrderHeaderRepository
	{
		private readonly ApplicationDbContext context;

		public OrderHeaderRepository(ApplicationDbContext context) : base(context)
		{
			this.context = context;
		}

		public void Update(OrderHeader orderHeader)
		{
			context.OrderHeaders.Update(orderHeader);
		}

		public void UpdateStatus(int id, string OrderStatus, string? PaymentStatus)
		{
			var orderHeader = context.OrderHeaders.FirstOrDefault(x => x.Id == id);
			if(orderHeader != null)
			{
				orderHeader.OrderStatus = OrderStatus;
				orderHeader.PaymentDate = DateTime.Now;
				if (PaymentStatus != null)
				{
					orderHeader.PaymentStatus = PaymentStatus;
				}
			}
		}
	}
}
