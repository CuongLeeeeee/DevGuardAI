using DevGuardAI.DAL.Data;
using DevGuardAI.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DevGuardAI.DAL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly DevGuardAIDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(DevGuardAIDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IQueryable<T> GetQueryable()
        {
            return _dbSet;
        }
        public async Task CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            // FindAsync can sometimes cause issues with non-standard key names or tracking.
            // Using FirstOrDefaultAsync is more reliable for finding by a specific property.
            var keyProperty = _context.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties.FirstOrDefault();
            if (keyProperty == null)
            {
                throw new InvalidOperationException($"Entity {typeof(T).Name} does not have a primary key defined.");
            }

            // This creates an expression tree: e => EF.Property<Guid>(e, "Id") == id
            var parameter = Expression.Parameter(typeof(T), "e");
            var property = Expression.Property(parameter, keyProperty.Name);
            var equals = Expression.Equal(property, Expression.Constant(id));
            var lambda = Expression.Lambda<Func<T, bool>>(equals, parameter);

            var entity = await _dbSet.FirstOrDefaultAsync(lambda);

            if (entity == null)
            {
                // We'll return null instead of throwing an exception to allow services to handle "not found" cases gracefully.
                return null;
            }

            return entity;
        }

        public async Task UpdateAsync(Guid id, T entity)
        {
            var existingEntity = await _dbSet.FindAsync(id);

            if (existingEntity == null)
            {
                throw new KeyNotFoundException(
                    $"Not found {typeof(T).Name} with Id: {id}");
            }

            _context.Entry(existingEntity)
            .CurrentValues
            .SetValues(entity);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var existingEntity = await _dbSet.FindAsync(id);
            if (existingEntity == null)
            {
                throw new KeyNotFoundException(
                    $"Not found {typeof(T).Name} with Id: {id}");
            }
            _dbSet.Remove(existingEntity);
            await _context.SaveChangesAsync();
        }

    }
}
