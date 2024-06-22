namespace Repository;

public interface ICrudBaseRepository<T, KeyType>
{
    IEnumerable<T> GetAll();
    T? GetById(KeyType id);
    T? Save(T entity);
    void DeleteById(KeyType id);
    void Update(T entity);
}
