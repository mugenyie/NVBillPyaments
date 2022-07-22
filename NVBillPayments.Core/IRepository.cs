using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NVBillPayments.Core
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Query();

        T GetById(Guid Id);

        Task AddAsync(T entity);

        void Add(T entity);

        void AddRange(IEnumerable<T> entity);

        IDbContextTransaction BeginTransaction();

        void SaveChanges();

        void Update(T entity);

        Task SaveChangesAsync();

        void Remove(T entity);
    }
}
