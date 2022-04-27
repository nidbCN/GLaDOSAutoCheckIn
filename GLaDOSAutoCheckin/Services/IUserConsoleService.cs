using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLaDOSAutoCheckin.Services
{
    public interface IUserConsoleService
    {
        public Task CheckIn();
        public Task Status();
    }
}
