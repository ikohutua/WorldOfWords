﻿using System.Collections.Generic;
using System.Linq;

namespace WorldOfWords.Infrastructure.Data.EF.Contracts
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        T GetById(int id);
        void Add(T entity);
        void Add(IEnumerable<T> entities);
        void Update(T entity);
        void Delete(T entity);
        void Delete(IEnumerable<T> entities);
        void Delete(int id);
    }
}
