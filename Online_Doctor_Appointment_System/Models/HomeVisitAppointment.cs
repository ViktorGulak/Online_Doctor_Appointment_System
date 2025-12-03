using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Online_Doctor_Appointment_System.Types;

namespace Online_Doctor_Appointment_System.Models
{
    class HomeVisitAppointment : Appointment // Выезд на дом к пациенту
    {
        private string patientAddress;
        private string city;
        private string streetTitle;
        private string houseNumber;
        private string appartmentNumber;
        public HomeVisitAppointment(long appointmentId, long patientId, long doctorId,DateTime dateTime, int duration, 
            AppointmentStatus status, string city, string streetTitle, string houseNumber, string appartmentNumber)
            : base(appointmentId, patientId, doctorId, dateTime, duration, status)
        {
            City = city;
            StreetTitle = streetTitle;
            HouseNumber = houseNumber;
            AppartmentNumber = appartmentNumber;
        }

        public string City
        {
            get => city;
            set => city = value;
        }

        public string StreetTitle
        {
            get => streetTitle;
            set => streetTitle = value;
        }

        public string HouseNumber
        {
            get => houseNumber;
            set => houseNumber = value;
        }

        public string AppartmentNumber
        {
            get => appartmentNumber;
            set => appartmentNumber = value;
        }

        public string PatientAddress => $"г. {City}, ул. {StreetTitle}, д. {HouseNumber}, вход {AppartmentNumber}";
         
        public override string GetDetails()
        {
            return "Приём на дому. Адрес: " + patientAddress;
        }

        public override string GetInfo()
        { 
            return $"{base.GetInfo()} Тип приёма: {GetDetails()}";
        }
    }
}
