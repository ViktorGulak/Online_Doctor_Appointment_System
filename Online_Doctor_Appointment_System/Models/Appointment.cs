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
        private Patient patient; // Только для кода, не для XML
        private long doctorId;
        private Doctor doctor;   // Только для кода, не для XML
        private DateTime appointmentDate;
        private TimeSpan startTime;
        private TimeSpan endTime;
        private string postscript;
        private AppointmentStatus status;

        public Appointment() { }

        public Appointment(long appointmentId, long patientId, Patient pat, long doctorId, Doctor doc,
                           DateTime appointmentDate, TimeSpan startTime, TimeSpan endTime,
                           string postscript, AppointmentStatus status)
        {
            AppointmentId = appointmentId;
            PatientId = patientId;
            RecordedPatient = pat;
            DoctorId = doctorId;
            ActingDoctor = doc;
            AppointmentDate = appointmentDate;
            StartTime = startTime;
            EndTime = endTime;
            Postscript = postscript;
            Status = status;
        }

        [XmlElement("AppointmentId")]
        public long AppointmentId
        {
            get => appointmentId;
            set => appointmentId = value;
        }

        [XmlElement("PatientId")]
        public long PatientId
        {
            get => patientId;
            set => patientId = value;
        }

        [XmlIgnore] // УБРАТЬ XmlElement! Patient хранится отдельно
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

        [XmlElement("DoctorId")]
        public long DoctorId
        {
            get => doctorId;
            set => doctorId = value;
        }

        [XmlIgnore] // УБРАТЬ XmlElement! Doctor хранится отдельно
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

        [XmlElement("AppointmentDate")]
        public DateTime AppointmentDate
        {
            get => appointmentDate;
            set => appointmentDate = value;
        }

        [XmlElement("StartTime")]
        public TimeSpan StartTime
        {
            get => startTime;
            set => startTime = value;
        }

        [XmlElement("EndTime")]
        public TimeSpan EndTime
        {
            get => endTime;
            set => endTime = value;
        }

        [XmlElement("Postscript")]
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

        [XmlIgnore]
        public string FormattedDate => AppointmentDate.ToString("dd.MM.yyyy");

        [XmlIgnore]
        public string FormattedStartTime => StartTime.ToString(@"hh\:mm");

        [XmlIgnore]
        public string FormattedEndTime => EndTime.ToString(@"hh\:mm");

        [XmlIgnore]
        public string FormattedTimeRange => $"{FormattedStartTime} - {FormattedEndTime}";

        [XmlIgnore]
        public string FormattedDateTime => $"{FormattedDate} {FormattedTimeRange}";

        [XmlIgnore]
        public abstract string AdditionalInfo { get; }

        public abstract string GetDetails();

        public virtual string GetInfo()
        {
            return $"ID: {AppointmentId}, " +
                   $"Пациент: {RecordedPatient?.FullName ?? PatientId.ToString()}, " +
                   $"Врач: {ActingDoctor?.FullName ?? DoctorId.ToString()}, " +
                   $"Дата/время: {FormattedDateTime}, " +
                   $"Статус: {Status}";
        }

        public bool TimeOverlapsWith(Appointment other)
        {
            if (this.AppointmentDate.Date != other.AppointmentDate.Date)
                return false;

            return (this.StartTime < other.EndTime && this.EndTime > other.StartTime);
        }

        public bool IsDoctorAvailable(List<Appointment> existingAppointments)
        {
            var doctorAppointments = existingAppointments
                .Where(a => a.DoctorId == this.DoctorId && a.Status != AppointmentStatus.CANCELLED)
                .ToList();

            return !doctorAppointments.Any(a => this.TimeOverlapsWith(a) && a.AppointmentId != this.AppointmentId);
        }
    }
}
