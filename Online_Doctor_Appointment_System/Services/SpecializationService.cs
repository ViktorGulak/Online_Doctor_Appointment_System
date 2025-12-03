using Online_Doctor_Appointment_System.Models;
using Online_Doctor_Appointment_System.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online_Doctor_Appointment_System.Services
{
    public class SpecializationService
    {
        private readonly IReadOnlyRepository<Specialization> _repository;

        public SpecializationService(IReadOnlyRepository<Specialization> repository)
        {
            _repository = repository;
        }

        // Получить все специальности
        public List<Specialization> GetAllSpecializations()
        {
            return _repository.GetAll();
        }

        // Получить специальность по ID
        public Specialization GetSpecializationById(long id)
        {
            return _repository.GetById(id);
        }

        // Получить специальность по названию
        public Specialization GetSpecializationByTitle(string title)
        {
            return _repository.GetAll()
                .FirstOrDefault(s => s.SpecializationTitle.Equals(title, StringComparison.OrdinalIgnoreCase));
        }

        // Получить список названий специальностей (для ComboBox)
        public List<string> GetSpecializationTitles()
        {
            return _repository.GetAll()
                .Select(s => s.SpecializationTitle)
                .ToList();
        }

        // Получить словарь ID -> Название (для привязки данных)
        public Dictionary<long, string> GetSpecializationDictionary()
        {
            return _repository.GetAll()
                .ToDictionary(s => s.SpecializationId, s => s.SpecializationTitle);
        }
    }
}
