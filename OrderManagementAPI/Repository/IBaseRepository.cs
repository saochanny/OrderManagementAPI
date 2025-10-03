namespace OrderManagementAPI.Repository;

public interface IBaseRepository<T> where T : class
{
    Task<T> SaveAsync(T entity);
    Task<T?> FindByIdAsync(object id);
    Task<IEnumerable<T>> FindAllAsync();
    Task DeleteAsync(T entity);
    Task DeleteByIdAsync(object id); // new method
}