using System.Data;

namespace Web_Service
{
    public interface IModelCreator<T>
    {
        public T CreateModel(IDataReader model);   
    }
}
