using BilheticaAeronauticaWeb.Entities;

namespace BilheticaAeronauticaWeb.Data
{

        public interface IGenericRepository<T> where T : class
        {
            IQueryable<T> GetAll();

            Task<T> GetByIdAsync(int id);

            Task CreateAsync(T entity);

            Task UpdateAsync(T entity);

            Task DeleteAsync(T entity);

            Task<bool> ExistAsync(int id);

            Task CreateRangeAsync(IEnumerable<T> entities);

            Task DeleteRangeAsync(IEnumerable<T> entities);

            Task UpdateRangeAsync(IEnumerable<T> entities);
    }
}
