﻿using BookCatalog.DataAccess.Data;
using BookCatalog.DataAccess.Repository.IRepository;
using BookCatalog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalog.DataAccess.Repository
{
	public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
	{
		private readonly ApplicationDbContext _db;
		public OrderHeaderRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}
		public void Update(OrderHeader orderHeader)
		{
			_db.OrderHeaders.Update(orderHeader);
		}

		public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
		{
			OrderHeader? orderHeader = _db.OrderHeaders.FirstOrDefault(oh => oh.Id == id);
			if (orderHeader is null) return;
			orderHeader.OrderStatus = orderStatus;
			if (!string.IsNullOrEmpty(paymentStatus))
			{
				orderHeader.PaymentStatus = paymentStatus;
			}
			_db.OrderHeaders.Update(orderHeader);
		}

		public void UpdateStripePaymentId(int id, string sessionId, string stripePaymentId)
		{
			OrderHeader? orderHeader = _db.OrderHeaders.FirstOrDefault(oh => oh.Id == id);
			if (orderHeader is null) return;
			if (!string.IsNullOrEmpty(sessionId))
			{
				orderHeader.SessionId = sessionId;
			}
			if (!string.IsNullOrEmpty(stripePaymentId))
			{
				orderHeader.PaymentIntentId = stripePaymentId;
				orderHeader.PaymentDate = DateTime.Now;
			}
			_db.OrderHeaders.Update(orderHeader);
		}
	}
}
