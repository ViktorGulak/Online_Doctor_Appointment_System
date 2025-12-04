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
    public class XmlPatientRepository : IRepository<Patient>
    {
        private readonly string _filePath;
        private readonly XmlSerializer _serializer;
        private List<Patient> _patients;

        public XmlPatientRepository()
        {
            _filePath = @"C:\Users\User\source\repos\Online_Doctor_Appointment_System\Online_Doctor_Appointment_System\Store\Patients.xml";

            // Указываем все типы для сериализации
            _serializer = new XmlSerializer(typeof(List<Patient>),
                new Type[] { typeof(Disease), typeof(Person) });

            _patients = LoadData();
        }

        private List<Patient> LoadData()
        {
            try
            {
                if (!File.Exists(_filePath))
                    return new List<Patient>();

                using (var stream = File.OpenRead(_filePath))
                {
                    var data = _serializer.Deserialize(stream) as List<Patient>;
                    return data ?? new List<Patient>();
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"Ошибка загрузки пациентов: {ex.Message}");
                return new List<Patient>();
            }
        }

        private void SaveData()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_filePath));

                using (var stream = File.Create(_filePath))
                {
                    _serializer.Serialize(stream, _patients);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ошибка сохранения пациентов: {ex.Message}");
            }
        }

        public List<Patient> GetAll() => new List<Patient>(_patients);

        public Patient GetById(long id) => _patients.FirstOrDefault(p => p.PersonId == id);

        public void Add(Patient patient)
        {
            if (patient == null)
                throw new ArgumentNullException(nameof(patient));

            // Проверка уникальности телефона
            if (_patients.Any(p => p.Phone == patient.Phone))
                throw new InvalidOperationException($"Пациент с телефоном {patient.Phone} уже существует");

            // Генерация ID пациента
            if (patient.PersonId == 0)
            {
                patient.PersonId = _patients.Count > 0 ? _patients.Max(p => p.PersonId) + 1 : 1;
            }

            // Генерация ID для болезни
            if (patient.PatientDisease != null && patient.PatientDisease.DiseaseId == 0)
            {
                long maxDiseaseId = _patients
                    .Where(p => p.PatientDisease != null)
                    .Select(p => p.PatientDisease.DiseaseId)
                    .DefaultIfEmpty(0)
                    .Max();

                patient.PatientDisease.DiseaseId = maxDiseaseId + 1;
                patient.DiseaseId = patient.PatientDisease.DiseaseId;
            }

            _patients.Add(patient);
            SaveData();
        }

        public void Update(Patient patient)
        {
            if (patient == null)
                throw new ArgumentNullException(nameof(patient));

            var existing = GetById(patient.PersonId);
            if (existing == null)
                throw new KeyNotFoundException($"Пациент с ID {patient.PersonId} не найден");

            // Проверка уникальности телефона
            if (_patients.Any(p => p.PersonId != patient.PersonId && p.Phone == patient.Phone))
                throw new InvalidOperationException($"Пациент с телефоном {patient.Phone} уже существует");

            var index = _patients.IndexOf(existing);
            _patients[index] = patient;
            SaveData();
        }

        public void Delete(long id)
        {
            var patient = GetById(id);
            if (patient == null)
                throw new KeyNotFoundException($"Пациент с ID {id} не найден");

            _patients.Remove(patient);
            SaveData();
        }

        public bool Exists(long id) => _patients.Any(p => p.PersonId == id);
    }
}
