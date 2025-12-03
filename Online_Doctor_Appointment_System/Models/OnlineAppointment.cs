using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online_Doctor_Appointment_System.Models
{
    class OnlineAppointment : Appointment // Онлайн консультация у врача
    {
        private string link;
        private string meetingService;
        private string chatCode;

        public OnlineAppointment(long appointmentId, long patientId, long doctorId,
            DateTime dateTime, int duration, AppointmentStatus status, string link, string meetingService, string chatCode)
            : base(appointmentId, patientId, doctorId, dateTime, duration, status)
        {
            Link = link;
            MeetingService = meetingService;
            ChatCode = chatCode;
        }

        public string Link
        {
            get => link;
            set => link = value;
        }
        public string MeetingService
        {
            get => meetingService;
            set => meetingService = value;
        }
        public string ChatCode
        {
            get => chatCode;
            set => chatCode = value;
        }

        public override string GetDetails()
        {
            return "Онлайн-приём. Ссылка на видеосвязь: " + Link;
        }

        public override string GetInfo()
        {
            return $"{base.GetInfo()} Тип приёма: {GetDetails()}";
        }
    }
}
