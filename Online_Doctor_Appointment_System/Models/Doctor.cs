using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Online_Doctor_Appointment_System.Models
{
    [Serializable]
    [XmlRoot("Doctor")]
    public class Doctor : Person
    {
        private long specializationId;
        private Specialization specialization;
        private string licenseNumber;
        private int experience;

        // Конструктор для сериализации
        public Doctor() : base() { }

        public Doctor(long personId, string name, string surname, string patronymic,
                     long specId, Specialization spec, string licNum, int exp)
            : base(personId, name, surname, patronymic)
        {
            SpecializationId = specId;
            DoctorSpecialization = spec;
            LicenseNumber = licNum;
            Experience = exp;
        }

        public long SpecializationId
        {
            get => specializationId;
            set => specializationId = value;
        }

        [XmlElement("Specialization")]
        public Specialization DoctorSpecialization
        {
            get => specialization;
            set
            {
                specialization = value;
                if (value != null)
                    specializationId = value.SpecializationId;
            }
        }

        public string LicenseNumber
        {
            get => licenseNumber;
            set
            {
                if (string.IsNullOrEmpty(value) || value.Length != 6)
                    throw new ArgumentException("Номер лицензии должен состоять из шести символов");
                licenseNumber = value;
            }
        }

        public int Experience
        {
            get => experience;
            set
            {
                if (value < 1 || value > 100)
                    throw new ArgumentException("Не допустимый диапозон чисел при указании стажа");
                experience = value;
            }
        }

        [XmlIgnore]
        public string FullName => $"{Surname} {Name} {Patronymic}";

        [XmlIgnore]
        public string DoctorProfession => $"{SpecializationTitle} {Surname} {Name[0]}.{Patronymic[0]}.";

        [XmlIgnore]
        public string SpecializationTitle => DoctorSpecialization?.SpecializationTitle ?? "Не указана";
    }
}
