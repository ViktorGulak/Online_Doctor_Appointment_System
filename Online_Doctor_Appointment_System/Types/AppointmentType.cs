using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online_Doctor_Appointment_System.Types
{
    // Перечисление для типа записи
    public enum AppointmentType
    {
        HomeVisit,      // Выезд на дом
        InPerson,       // Очный прием
        Online          // Онлайн
    }
}
