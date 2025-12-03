using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online_Doctor_Appointment_System.Types
{
    public interface IReadOnlyRepository<T> where T : class
    {
        List<T> GetAll();
        T GetById(long id);
    }
}
