using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online_Doctor_Appointment_System.Types
{
    public interface IRepository<T> where T : class
    {
        List<T> GetAll();
        T GetById(long id);
        void Add(T item);
        void Update(T item);
        void Delete(long id);
        bool Exists(long id);
    }
}
