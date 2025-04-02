namespace ErpProject.Repository.Basic
{
    public interface IRepository<T> where T : class
    { 
        public Task<T> GetByIdAsync(int id);
        public Task<IEnumerable<T>> GetAllAsync();
        public Task AddAsync(T entity);
        public void Delete(T entity);
        public void Update(T entity);
    }
}
