using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CH.CleanArchitecture.Infrastructure.Services
{
    public interface IDbInitializerService
    {
        void Migrate();
        void Seed();
    }
}
