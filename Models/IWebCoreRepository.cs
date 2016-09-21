using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webcore.Models
{
    public interface IWebCoreRepository
    {
        Task<List<T>> GetRecordsTaskAsync<T>() where T : class;
        Task<int> InsertRecordTaskAsync<T>(T entity) where T : class;
    }
}
