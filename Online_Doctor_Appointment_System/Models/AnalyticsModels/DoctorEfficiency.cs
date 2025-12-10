using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online_Doctor_Appointment_System.Models.AnalyticsModels
{
    public class DoctorEfficiency
    {
        public Doctor Doctor { get; set; }
        public double EfficiencyScore { get; set; } // 0-100 баллов
        public double SuccessRate { get; set; } // Процент успешных записей
        public double AverageDuration { get; set; } // Средняя продолжительность приема
        public int TotalAppointments { get; set; }
        public int SuccessfulAppointments { get; set; }
    }
}
