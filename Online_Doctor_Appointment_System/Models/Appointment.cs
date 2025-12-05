using Online_Doctor_Appointment_System.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Online_Doctor_Appointment_System.Models
{
    [Serializable]
    [XmlRoot("Appointment")]
    [XmlInclude(typeof(HomeVisitAppointment))]
    [XmlInclude(typeof(InPersonAppointment))]
    [XmlInclude(typeof(OnlineAppointment))]
    public abstract class Appointment : IMedicalAppointment
    {
        private long appointmentId;
        private long patientId;
        private Patient patient;
        private long doctorId;
        private Doctor doctor;
        private DateTime appointmentDateTime; // Объединенная дата и время
        private TimeSpan startTime;
        private TimeSpan endTime;
        private string postscript;
        private AppointmentStatus status;

        public Appointment() { }

        public Appointment(long appointmentId, long patientId, Patient pat, long doctorId, Doctor doc,
                           DateTime appointmentDateTime, TimeSpan startTime, TimeSpan endTime,
                           string postscript, AppointmentStatus status)
        {
            AppointmentId = appointmentId;
            PatientId = patientId;
            RecordedPatient = pat;
            DoctorId = doctorId;
            ActingDoctor = doc;
            AppointmentDateTime = appointmentDateTime;
            StartTime = startTime;
            EndTime = endTime;
            Postscript = postscript;
            Status = status;
        }

        public long AppointmentId
        {
            get => appointmentId;
            set
            {
                if (value < 0) throw new ArgumentException("Идентификатор записи не может быть меньше 0.");
                appointmentId = value;
            }
        }

        public long PatientId
        {
            get => patientId;
            set
            {
                if (value < 0) throw new ArgumentException("Идентификатор пациента не может быть меньше 0.");
                patientId = value;
            }
        }

        [XmlElement("Patient")]
        public Patient RecordedPatient
        {
            get => patient;
            set
            {
                patient = value;
                if (value != null)
                    PatientId = value.PersonId;
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

        [XmlElement("Doctor")]
        public Doctor ActingDoctor
        {
            get => doctor;
            set
            {
                doctor = value;
                if (value != null)
                    DoctorId = value.PersonId;
            }
        }

        [XmlElement("AppointmentDateTime")]
        public DateTime AppointmentDateTime
        {
            get => appointmentDateTime;
            set
            {
                DateTime currentDate = DateTime.Now;
                DateTime minDate = currentDate.AddYears(-1); // Можно смотреть записи за прошлый год
                DateTime maxDate = currentDate.AddMonths(3); // Записи на 3 месяца вперед

                if (value < minDate)
                    throw new ArgumentException($"Дата записи не может быть раньше {minDate:dd.MM.yyyy}");

                if (value > maxDate)
                    throw new ArgumentException($"Дата записи не может быть позже {maxDate:dd.MM.yyyy}");

                appointmentDateTime = value.Date; // Сохраняем только дату
            }
        }

        [XmlElement("StartTime")]
        public TimeSpan StartTime
        {
            get => startTime;
            set
            {
                // Проверка что время в пределах дня (0-23:59)
                if (value < TimeSpan.Zero || value >= TimeSpan.FromHours(24))
                    throw new ArgumentException("Время начала должно быть между 00:00 и 23:59");

                // Проверка что время начала раньше времени окончания
                if (value >= EndTime)
                    throw new ArgumentException("Время начала должно быть раньше времени окончания");

                startTime = value;
            }
        }

        [XmlElement("EndTime")]
        public TimeSpan EndTime
        {
            get => endTime;
            set
            {
                // Проверка что время в пределах дня
                if (value < TimeSpan.Zero || value >= TimeSpan.FromHours(24))
                    throw new ArgumentException("Время окончания должно быть между 00:00 и 23:59");

                // Проверка что время окончания позже времени начала
                if (value <= StartTime)
                    throw new ArgumentException("Время окончания должно быть позже времени начала");

                // Проверка что прием не длится больше 4 часов (можно изменить)
                if ((value - StartTime) > TimeSpan.FromHours(4))
                    throw new ArgumentException("Продолжительность приема не может превышать 4 часа");

                endTime = value;
            }
        }

        [XmlIgnore]
        public TimeSpan Duration => EndTime - StartTime;

        public string Postscript
        {
            get => postscript;
            set => postscript = value;
        }

        [XmlElement("Status")]
        public AppointmentStatus Status
        {
            get => status;
            set => status = value;
        }

        // Форматированные свойства для отображения
        [XmlIgnore]
        public string FormattedDate => AppointmentDateTime.ToString("dd.MM.yyyy");

        [XmlIgnore]
        public string FormattedStartTime => StartTime.ToString(@"hh\:mm");

        [XmlIgnore]
        public string FormattedEndTime => EndTime.ToString(@"hh\:mm");

        [XmlIgnore]
        public string FormattedTimeRange => $"{FormattedStartTime} - {FormattedEndTime}";

        [XmlIgnore]
        public string FormattedDateTime => $"{FormattedDate} {FormattedTimeRange}";

        // Для удобства - полная дата+время начала
        [XmlIgnore]
        public DateTime FullStartDateTime => AppointmentDateTime.Add(StartTime);

        [XmlIgnore]
        public DateTime FullEndDateTime => AppointmentDateTime.Add(EndTime);

        // Абстрактный метод из интерфейса
        public abstract string GetDetails();

        // Метод для получения полной информации
        public virtual string GetInfo()
        {
            return $"ID: {appointmentId}, " +
                   $"Пациент: {RecordedPatient?.FullName ?? PatientId.ToString()}, " +
                   $"Врач: {ActingDoctor?.FullName ?? DoctorId.ToString()}, " +
                   $"Дата/время: {FormattedDateTime}, " +
                   $"Статус: {Status}";
        }

        // Метод для проверки пересечения времени с другой записью
        public bool TimeOverlapsWith(Appointment other)
        {
            if (this.AppointmentDateTime.Date != other.AppointmentDateTime.Date)
                return false;

            return (this.StartTime < other.EndTime && this.EndTime > other.StartTime);
        }

        // Метод для проверки доступности врача в это время
        public bool IsDoctorAvailable(List<Appointment> existingAppointments)
        {
            var doctorAppointments = existingAppointments
                .Where(a => a.DoctorId == this.DoctorId && a.Status != AppointmentStatus.CANCELLED)
                .ToList();

            return !doctorAppointments.Any(a => this.TimeOverlapsWith(a) && a.AppointmentId != this.AppointmentId);
        }
    }
}
