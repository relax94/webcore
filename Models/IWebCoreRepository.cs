using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webcore.Models
{
    public interface IWebCoreRepository
    {
        Task<IEnumerable<Trip>> GetAllTrips<T>();
    }
}
