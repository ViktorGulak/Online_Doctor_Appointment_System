using Online_Doctor_Appointment_System.Models;
using Online_Doctor_Appointment_System.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Online_Doctor_Appointment_System.Repositories
{
    public class XmlDoctorRepository : IRepository<Doctor>
    {
        private readonly string _filePath;
        private readonly XmlSerializer _serializer;
        private List<Doctor> _doctors;
        private readonly SpecializationRepository _specRepository;

        public XmlDoctorRepository()
        {
            _filePath = @"C:\Users\User\source\repos\Online_Doctor_Appointment_System\Online_Doctor_Appointment_System\Store\Doctors.xml";

            _serializer = new XmlSerializer(typeof(List<Doctor>),
                new Type[] { typeof(Specialization), typeof(Person) });

            _specRepository = new SpecializationRepository();

            _doctors = LoadData();
            LinkSpecializations(); // Связываем врачей со специальностями
        }

        private List<Doctor> LoadData()
        {
            try
            {
                if (!File.Exists(_filePath))
                    return new List<Doctor>();

                using (var stream = File.OpenRead(_filePath))
                {
                    var data = _serializer.Deserialize(stream) as List<Doctor>;
                    return data ?? new List<Doctor>();
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"Ошибка загрузки врачей: {ex.Message}");
                return new List<Doctor>();
            }
        }

        // Связываем врачей с объектами Specialization из общего файла
        private void LinkSpecializations()
        {
            var allSpecializations = _specRepository.GetAll();

            foreach (var doctor in _doctors)
            {
                // Если у врача есть SpecializationId, ищем соответствующую специальность
                if (doctor.SpecializationId > 0)
                {
                    var spec = allSpecializations.FirstOrDefault(s => s.SpecializationId == doctor.SpecializationId);
                    if (spec != null)
                    {
                        doctor.DoctorSpecialization = spec;
                    }
                    else if (doctor.DoctorSpecialization == null)
                    {
                        // Если специальность не найдена, создаем пустую
                        doctor.DoctorSpecialization = new Specialization
                        {
                            SpecializationId = doctor.SpecializationId,
                            SpecializationTitle = "Неизвестная",
                            SpecializationDescription = "Специальность не найдена"
                        };
                    }
                }
            }
        }

        private void SaveData()
        {
            try
            {
                // Перед сохранением обновляем SpecializationId из объекта
                foreach (var doctor in _doctors)
                {
                    if (doctor.DoctorSpecialization != null)
                    {
                        doctor.SpecializationId = doctor.DoctorSpecialization.SpecializationId;
                    }
                }

                using (var stream = File.Create(_filePath))
                {
                    _serializer.Serialize(stream, _doctors);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ошибка сохранения врачей: {ex.Message}");
            }
        }

        public List<Doctor> GetAll() => new List<Doctor>(_doctors);

        public Doctor GetById(long id) => _doctors.FirstOrDefault(d => d.PersonId == id);

        public void Add(Doctor doctor)
        {
            if (doctor == null)
                throw new ArgumentNullException(nameof(doctor));

            // Проверка уникальности лицензии
            if (_doctors.Any(d => d.LicenseNumber == doctor.LicenseNumber))
                throw new InvalidOperationException($"Врач с лицензией {doctor.LicenseNumber} уже существует");

            // Генерация ID
            if (doctor.PersonId == 0)
            {
                doctor.PersonId = _doctors.Count > 0 ? _doctors.Max(d => d.PersonId) + 1 : 1;
            }

            _doctors.Add(doctor);
            SaveData();
        }

        public void Update(Doctor doctor)
        {
            if (doctor == null)
                throw new ArgumentNullException(nameof(doctor));

            var existing = GetById(doctor.PersonId);
            if (existing == null)
                throw new KeyNotFoundException($"Врач с ID {doctor.PersonId} не найден");

            // Проверка уникальности лицензии
            if (_doctors.Any(d => d.PersonId != doctor.PersonId && d.LicenseNumber == doctor.LicenseNumber))
                throw new InvalidOperationException($"Врач с лицензией {doctor.LicenseNumber} уже существует");

            var index = _doctors.IndexOf(existing);
            _doctors[index] = doctor;
            SaveData();
        }

        public void Delete(long id)
        {
            var doctor = GetById(id);
            if (doctor == null)
                throw new KeyNotFoundException($"Врач с ID {id} не найден");

            _doctors.Remove(doctor);
            SaveData();
        }

        public bool Exists(long id) => _doctors.Any(d => d.PersonId == id);
    }
}
