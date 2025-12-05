using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Online_Doctor_Appointment_System.Models
{
    [Serializable]
    [XmlRoot("InPersonAppointment")]
    public class InPersonAppointment : Appointment
    {
        
        private string hospitalTitle;
        private string cabinetNumber;

        public InPersonAppointment() : base() { }

        public InPersonAppointment(long appointmentId, long patientId, Patient patient, long doctorId, Doctor doctor,
            DateTime appointmentDateTime, TimeSpan startTime, TimeSpan endTime, string postscript,
            AppointmentStatus status, string hospitalTitle, string cabinetNumber)
            : base(appointmentId, patientId, patient, doctorId, doctor, appointmentDateTime, startTime, endTime,
                   postscript, status)
        {
            HospitalTitle = hospitalTitle;
            CabinetNumber = cabinetNumber;
        }

        [XmlElement("HospitalTitle")]
        public string HospitalTitle
        {
            get => hospitalTitle;
            set => hospitalTitle = value;
        }

        [XmlElement("CabinetNumber")]
        public string CabinetNumber
        {
            get => cabinetNumber;
            set => cabinetNumber = value;
        }

        [XmlIgnore]
        public string ReceptionLocation => $"Больница {HospitalTitle}, каб. {CabinetNumber}";

        public override string GetDetails()
        {
            return $"Очный приём. Место: {ReceptionLocation}";
        }

        public override string GetInfo()
        {
            return $"{base.GetInfo()}. Тип: {GetDetails()}";
        }
    }

}
