using SHIVAM_ECommerce.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SHIVAM_ECommerce.Repository
{

    public class Repository<T> : IRepository<T> where T : class
    {
        private ApplicationDbContext db;
        private DbSet<T> dbSet;

        public Repository()
        {
            db = new ApplicationDbContext();
            dbSet = db.Set<T>();
        }
        public IEnumerable<T> GetAll()
        {
            return dbSet.ToList();
        }

        public T GetById(object Id)
        {
            return dbSet.Find(Id);
        }

        public void Insert(T obj)
        {
            dbSet.Add(obj);
        }
        public void Update(T obj)
        {
            db.Entry(obj).State = EntityState.Modified;
        }
        public void Delete(object Id)
        {
            T getObjById = dbSet.Find(Id);
            dbSet.Remove(getObjById);
        }
        public void Save()
        {
            db.SaveChanges();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.db != null)
                {
                    this.db.Dispose();
                    this.db = null;
                }
            }
        }

        
        public IEnumerable<ProductImages> GetProductImages(int ProductID)
        {
            return db.ProductImages.Where(x => x.ProductQuantityId == ProductID).ToList();
        }



        public bool DeleteAttributeForSupplier(int SupplierID)
        {
            try
            {
                var allItems=db.ProductAttributesRelation.Where(p=>p.SupplierID==SupplierID).Select(p=>p);
                db.ProductAttributesRelation.RemoveRange(allItems);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                
            }
        }
    }
}