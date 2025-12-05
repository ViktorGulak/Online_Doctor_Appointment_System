using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Online_Doctor_Appointment_System.Models
{
    [Serializable]
    [XmlRoot("OnlineAppointment")]
    public class OnlineAppointment : Appointment
    {
        private string link;
        private string chatCode;

        public OnlineAppointment() : base() { }

        public OnlineAppointment(long appointmentId, long patientId, Patient patient, long doctorId, Doctor doctor,
            DateTime appointmentDateTime, TimeSpan startTime, TimeSpan endTime, string postscript,
            AppointmentStatus status, string link, string chatCode)
            : base(appointmentId, patientId, patient, doctorId, doctor, appointmentDateTime, startTime, endTime,
                   postscript, status)
        {
            Link = link;
            ChatCode = chatCode;
        }

        [XmlElement("Link")]
        public string Link
        {
            get => link;
            set => link = value;
        }

        [XmlElement("ChatCode")]
        public string ChatCode
        {
            get => chatCode;
            set => chatCode = value;
        }

        [XmlIgnore]
        public string VideoConferenceInfo => $"Ссылка: {Link}, Код чата: {ChatCode}";

        public override string GetDetails()
        {
            return $"Онлайн-приём. {VideoConferenceInfo}";
        }

        public override string GetInfo()
        {
            return $"{base.GetInfo()}. Тип: {GetDetails()}";
        }
    }
}
