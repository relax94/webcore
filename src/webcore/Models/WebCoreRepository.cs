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
        
        public async Task<List<T>> GetRecordsTaskAsync<T>() where T : class
        {
            return await Task.Factory.StartNew(() => this._context.Set<T>().ToList<T>());
        }

        public async Task<int> InsertRecordTaskAsync<T>(T entity) where T : class
        {
            await Task.Factory.StartNew(() => this._context.Set<T>().Add(entity));
            return this._context.SaveChanges();
        }
    }
}
