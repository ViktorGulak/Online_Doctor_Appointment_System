using Online_Doctor_Appointment_System.Models;
using Online_Doctor_Appointment_System.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace Online_Doctor_Appointment_System.Repositories
{
    public class SpecializationRepository : IReadOnlyRepository<Specialization>
    {
        private readonly string _filePath;
        private readonly XmlSerializer _serializer;
        private List<Specialization> _specializations;

        public SpecializationRepository()
        {
            // Путь к файлу
            _filePath = @"C:\Users\User\source\repos\Online_Doctor_Appointment_System\Online_Doctor_Appointment_System\Store\Specializations.xml";

            // Сериализатор для List<Specialization>
            _serializer = new XmlSerializer(typeof(List<Specialization>));

            // Загружаем данные при создании
            _specializations = LoadData();
        }

        private List<Specialization> LoadData()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    // Если файла нет - возвращаем пустой список
                    return new List<Specialization>();
                }

                using (var stream = File.OpenRead(_filePath))
                {
                    var data = _serializer.Deserialize(stream) as List<Specialization>;
                    return data ?? new List<Specialization>();
                }
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем пустой список
                throw new InvalidOperationException($"Ошибка загрузки специальностей: {ex.Message}");
                //return new List<Specialization>();
            }
        }

        // Получить все специальности
        public List<Specialization> GetAll()
        {
            return new List<Specialization>(_specializations); // Возвращаем копию
        }

        // Найти специальность по ID
        public Specialization GetById(long id)
        {
            return _specializations.FirstOrDefault(s => s.SpecializationId == id);
        }

        // Найти специальность по названию
        public Specialization GetByTitle(string title)
        {
            return _specializations.FirstOrDefault(s =>
                s.SpecializationTitle.Equals(title, StringComparison.OrdinalIgnoreCase));
        }
    }
}
