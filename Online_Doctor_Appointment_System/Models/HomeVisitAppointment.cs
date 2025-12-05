using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Online_Doctor_Appointment_System.Types;

namespace Online_Doctor_Appointment_System.Models
{
    [Serializable]
    [XmlRoot("HomeVisitAppointment")]
    public class HomeVisitAppointment : Appointment
    {
        private string city;
        private string streetTitle;
        private string houseNumber;
        private string appartmentNumber;

        public HomeVisitAppointment() : base() { }

        public HomeVisitAppointment(long appointmentId, long patientId, Patient patient, long doctorId, Doctor doctor,
            DateTime appointmentDateTime, TimeSpan startTime, TimeSpan endTime, string postscript,
            AppointmentStatus status, string city, string streetTitle, string houseNumber, string appartmentNumber)
            : base(appointmentId, patientId, patient, doctorId, doctor, appointmentDateTime, startTime, endTime,
                   postscript, status)
        {
            City = city;
            StreetTitle = streetTitle;
            HouseNumber = houseNumber;
            AppartmentNumber = appartmentNumber;
        }

        [XmlElement("City")]
        public string City
        {
            get => city;
            set => city = value;
        }

        [XmlElement("StreetTitle")]
        public string StreetTitle
        {
            get => streetTitle;
            set => streetTitle = value;
        }

        [XmlElement("HouseNumber")]
        public string HouseNumber
        {
            get => houseNumber;
            set => houseNumber = value;
        }

        [XmlElement("AppartmentNumber")]
        public string AppartmentNumber
        {
            get => appartmentNumber;
            set => appartmentNumber = value;
        }

        [XmlIgnore]
        public string PatientAddress => $"г. {City}, ул. {StreetTitle}, д. {HouseNumber}, кв. {AppartmentNumber}";

        [XmlIgnore]
        public string AddressForMap => $"{City}, {StreetTitle}, {HouseNumber}";

        public override string GetDetails()
        {
            return $"Приём на дому. Адрес: {PatientAddress}";
        }

        public override string GetInfo()
        {
            return $"{base.GetInfo()}. Тип: {GetDetails()}";
        }
    }
}
