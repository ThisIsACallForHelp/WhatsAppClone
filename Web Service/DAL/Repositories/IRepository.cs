namespace Web_Service
{
    public interface IRepository<T>
    {
        T GetByID(string ID);
        List<T> GetAll();
        bool Create(T entity);
        bool Update(T entity);
        bool Delete(T entity);
        bool DeleteByID(string ID);
    }
}
