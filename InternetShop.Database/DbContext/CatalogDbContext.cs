﻿using InternetShop.Domain;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetShop.Database
{
    public class CatalogDbContext
    {
        public IMongoCollection<Product> Products { get; }

        public CatalogDbContext(IMongoDatabase database)
        {
            Products = database.GetCollection<Product>("products");
            CreateIndexes();
        }

        private void CreateIndexes()
        {
            // Только технические индексы
            Products.Indexes.CreateOne(
                new CreateIndexModel<Product>(
                    Builders<Product>.IndexKeys.Ascending(p => p.Categories)));
        }
    }
}