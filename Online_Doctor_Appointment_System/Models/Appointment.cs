using Online_Doctor_Appointment_System.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online_Doctor_Appointment_System.Models
{
    abstract class Appointment : IMedicalAppointment
    {
        private long appointmentId;
        private long patientId;
        private long doctorId;
        private DateTime appointmentDate;
        private int duration; // Продолжительность приёма в минутах
        private AppointmentStatus status;

        public Appointment(long appointmentId, long patientId, long doctorId,
                           DateTime date, int duration, AppointmentStatus status)
        {
            AppointmentId = appointmentId;
            PatientId = patientId;
            DoctorId = doctorId;
            AppointmentDate = date;
            Duration = duration;
            Status = status;
        }

        public long AppointmentId { 
            get => appointmentId; 
            set 
            {
                if(value < 0) throw new ArgumentException("Идентификатор записи не может быть меньше 0.");
                appointmentId = value;
            } 
        }

        public long PatientId { 
            get => patientId;
            set
            {
                if (value < 0) throw new ArgumentException("Идентификатор пациента не может быть меньше 0.");
                patientId = value;
            }
        }

        public long DoctorId
        {
            get => doctorId;
            set
            {
                if (value < 0) throw new ArgumentException("Идентификатор врача не может быть меньше 0.");
                doctorId = value;
            }
        }

        public DateTime AppointmentDate
        {
            get => appointmentDate;
            set
            {
                DateTime currentDate = DateTime.Now.Date;
                DateTime minDate = currentDate.AddYears(-10);
                DateTime maxDate = currentDate.AddDays(30);

                if (value < minDate)
                    throw new ArgumentException($"Дата записи не может быть раньше {minDate:dd.MM.yyyy}");

                if (value > maxDate)
                    throw new ArgumentException($"Дата записи не может быть позже {maxDate:dd.MM.yyyy}");

                appointmentDate = value;
            }
        }

        public int Duration
        {
            get => duration;
            set
            {
                if (value < 0 || value > 120) throw new ArgumentException("Продолжительность приёма не может быть меньше 0 и больше 120 минут.");
                duration = value;
            }
        }

        public string FormattedAppointmentDate => appointmentDate.ToString("dd.MM.yyyy");

        public AppointmentStatus Status
        {
            get => status;
            set => status = value;
        }
        // Абстрактный метод из интерфейса
        public abstract string GetDetails();

        // Метод для получения полной информации
        public virtual string GetInfo()
        {
            return $"ID: {appointmentId},\nПациент: {patientId},\nВрач: {doctorId},\nДата: {appointmentDate},\nСтатус: {status}";
        }
    }
}
