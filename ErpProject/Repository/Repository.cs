using ErpProject.Data;
using ErpProject.Repository.Basic;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace ErpProject.Repository
{
    public class Repository<T>:IRepository<T> where T : class
    {
        private readonly ErpDbContext _context; 
        public Repository(ErpDbContext contex) 
        {
            _context = contex;
        }
        public async Task<T> GetByIdAsync(int id)
        {
            T entity=await _context.Set<T>().FindAsync(id);  
            return entity;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var entitys=await _context.Set<T>().ToListAsync();
            return entitys;
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            _context.SaveChanges();
        }
        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
            _context.SaveChanges();
        }
    }
}
