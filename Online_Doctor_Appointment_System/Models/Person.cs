using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online_Doctor_Appointment_System.Models
{
    public abstract class Person
    {
        private long id;
        private string name;
        private string surname;
        private string patronymic;

        // Конструктор для сериализации
        protected Person() { }

        public Person(long personId, string name, string surname, string patronymic)
        {
            PersonId = personId;            
            Name = name;             
            Surname = surname;         
            Patronymic = patronymic;
        }

        public long PersonId
        {
            get => id;
            set
            {
                if (value < 0) throw new ArgumentException("Идентификатор не может быть меньше 0.");
                id = value;
            }
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public string Surname
        {
            get => surname;
            set => surname = value;
        }

        public string Patronymic
        {
            get => patronymic;
            set => patronymic = value;
        }

        //protected string FullName => $"{Surname}, {Name}, {Patronymic}";
    }
}
