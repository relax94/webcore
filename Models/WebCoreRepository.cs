using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webcore.Models
{
    public class WebCoreRepository : IWebCoreRepository
    {
        private WebCoreContext _context;
        private ILogger<WebCoreRepository> _logger;

        public WebCoreRepository(WebCoreContext context, ILogger<WebCoreRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Trip>> GetAllTrips<T>()
        {
            try
            {
                throw new Exception("OOOOOOOOOOOOOOOOOOOOOOOOOOO");
                return await Task.Factory.StartNew(() => _context.Trips.ToList());
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

    }
}
