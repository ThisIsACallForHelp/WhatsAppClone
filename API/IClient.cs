using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API
{
    public interface IClient<T>
    {
        Task<T> GetAsync();
        Task<bool> PostAsync(T model);
    }
}
