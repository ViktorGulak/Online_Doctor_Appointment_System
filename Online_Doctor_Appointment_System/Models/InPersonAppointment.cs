using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online_Doctor_Appointment_System.Models
{
    class InPersonAppointment : Appointment // Очный приём в поликлинике
    {
        private string city;
        private string streetTitle;
        private string houseNumber;
        private string hospitalTitle;
        private string cabinetNumber;

        public InPersonAppointment(long appointmentId, long patientId, long doctorId, DateTime dateTime, int duration, 
            AppointmentStatus status, string city, string streetTitle, string houseNumber, string hospitalTitle, string cabinetNumber)
            : base(appointmentId, patientId, doctorId, dateTime, duration, status)
        {

            City = city;
            StreetTitle = streetTitle;
            HouseNumber = houseNumber;
            HospitalTitle = hospitalTitle;
            CabinetNumber = cabinetNumber;
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

        public string HospitalTitle
        {
            get => hospitalTitle;
            set => hospitalTitle = value;
        }

        public string CabinetNumber
        {
            get => cabinetNumber;
            set => cabinetNumber = value;
        }

        public string ReceptionLocation => $"г. {City}, ул. {StreetTitle}, д. {HouseNumber}, {HospitalTitle}, кабинет № {CabinetNumber}";
        public override string GetDetails()
        {
            return "Очный приём. Место проведения " + ReceptionLocation;
        }

        public override string GetInfo()
        { 
            return $"{base.GetInfo()} Тип приёма: {GetDetails()}";
        }
    }
}
