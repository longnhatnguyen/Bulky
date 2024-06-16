using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(string? includeProperties = null);
		T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = true);

		T Get(Expression<Func<T, bool>> fillter);
        void Update(T entity);
        void Add(T entity);
        void RemoveRange(IEnumerable<T> entities);
        void Remove(T entity);
        void Save();
    }
}
