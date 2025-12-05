using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online_Doctor_Appointment_System
{
    public enum AppointmentStatus
    {
        SCHEDULED, // Запланирован
        COMPLETED, // Выполнен
        CANCELLED, // Отменён
        NO_SHOW // Не явка
    }
}
