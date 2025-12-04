using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Online_Doctor_Appointment_System.Models
{
    [Serializable]
    [XmlRoot("Patient")]
    public class Patient : Person
    {
        private string phone;
        private string email;
        private DateTime dateOfBirth;
        private long diseaseId;
        private Disease disease;

        // Конструктор для сериализации (обязательно)
        public Patient() : base() { }
        public Patient(long personId, string name, string surname, string patronymic, string phone,
            string email, DateTime dateOfBirth, long diseaseId, Disease disease)
            : base(personId, name, surname, patronymic)
        {
            Phone = phone;
            Email = email;
            DateOfBirth = dateOfBirth;
            DiseaseId = diseaseId;
            PatientDisease = disease;
        }

        public string Phone
        {
            get => phone;
            set
            {
                if (value.Length < 0 || value.Length > 11) throw new ArgumentException("Номер телефона должен состоять из 11 цифр");
                phone = value;
            }
        }

        public string Email
        {
            get => email;
            set => email = value;
        }

        public DateTime DateOfBirth
        {
            get => dateOfBirth;
            set
            {
                DateTime currentDate = DateTime.Now.Date;
                DateTime minDate = new DateTime(1930, 1, 1);
                DateTime maxDate = new DateTime(currentDate.Year, 12, 31);

                if (value < minDate)
                    throw new ArgumentException($"Дата рождения не может быть раньше {minDate:dd.MM.yyyy}");

                if (value > maxDate)
                    throw new ArgumentException($"Дата рождения не может быть позже {maxDate:dd.MM.yyyy}");

                dateOfBirth = value;
            }
        }

        public long DiseaseId
        {
            get => diseaseId;
            set
            {
                if (value < 0) throw new ArgumentException("Идентификатор болезни не может быть меньше 0.");
                diseaseId = value;
            }
        }

        [XmlElement("Disease")]
        public Disease PatientDisease
        {
            get => disease;
            set => disease = value;
        }

        [XmlIgnore]
        public string FormattedDateOfBirth => dateOfBirth.ToString("dd.MM.yyyy");

        [XmlIgnore]
        public string FullName => $"{Surname} {Name} {Patronymic}";

        [XmlIgnore]
        public string DiseaseTitle => PatientDisease?.DiseaseTitle ?? "Не указана";

    }
}
