﻿using BookCatalog.DataAccess.Data;
using BookCatalog.DataAccess.Repository.IRepository;
using BookCatalog.Models;

namespace BookCatalog.DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext _db;
        public ShoppingCartRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(ShoppingCart shoppingCart)
        {
            _db.ShoppingCarts.Update(shoppingCart);
        }
        public override void Add(ShoppingCart entity)
        {
            ShoppingCart? duplicate = Get(sc => sc.ProductId == entity.ProductId && sc.ApplicationUserId == entity.ApplicationUserId, true);
            if (duplicate is null) 
            {
                base.Add(entity);
                return;
            }
            duplicate.Count += entity.Count;
            
        }
    }
}
