using SHIVAM_ECommerce.Models;
using System;
using System.Collections.Generic;

namespace SHIVAM_ECommerce.Repository
{
    public interface IRepository<T> where T: class
    {
        IEnumerable<T> GetAll();
        T GetById(object Id);
        void Insert(T obj);
        void Update(T obj);
        void Delete(Object Id);
        void Save();
        bool DeleteAttributeForSupplier(int SupplierID);
        IEnumerable<ProductImages> GetProductImages(int ProductID);
    }
}