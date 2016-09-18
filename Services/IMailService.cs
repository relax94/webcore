using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webcore.Services
{
    public interface IMailService
    {
        void SendMail(string from, string to, string subject, string message);
    }
}
