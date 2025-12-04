using Online_Doctor_Appointment_System.Models;
using Online_Doctor_Appointment_System.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online_Doctor_Appointment_System.Services
{
    public class PatientService
    {
        private readonly IRepository<Patient> _patientRepository;

        public PatientService(IRepository<Patient> patientRepository)
        {
            _patientRepository = patientRepository;
        }

        // Базовые CRUD операции
        public List<Patient> GetAllPatients() => _patientRepository.GetAll();
        public Patient GetPatientById(long id) => _patientRepository.GetById(id);
        public void AddPatient(Patient patient) => _patientRepository.Add(patient);
        public void UpdatePatient(Patient patient) => _patientRepository.Update(patient);
        public void DeletePatient(long id) => _patientRepository.Delete(id);

        // Поиск пациентов (перенесли из репозитория в сервис)
        public List<Patient> SearchPatients(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAllPatients();

            var term = searchTerm.ToLowerInvariant();
            var allPatients = GetAllPatients();

            return allPatients.Where(p =>
            {
                // Поиск по ФИО
                if (p.Name.ToLowerInvariant().Contains(term) ||
                    p.Surname.ToLowerInvariant().Contains(term) ||
                    p.Patronymic.ToLowerInvariant().Contains(term) ||
                    p.FullName.ToLowerInvariant().Contains(term))
                    return true;

                // Поиск по телефону и email
                if (p.Phone.Contains(term) ||
                    (p.Email != null && p.Email.ToLowerInvariant().Contains(term)))
                    return true;

                // Поиск по дате рождения
                if (p.FormattedDateOfBirth.Contains(term))
                    return true;

                // Поиск по болезни
                if (p.PatientDisease != null)
                {
                    if ((p.PatientDisease.DiseaseTitle != null &&
                         p.PatientDisease.DiseaseTitle.ToLowerInvariant().Contains(term)) ||
                        (p.PatientDisease.DiseaseSymptom != null &&
                         p.PatientDisease.DiseaseSymptom.ToLowerInvariant().Contains(term)))
                        return true;
                }

                return false;
            }).ToList();
        }

        // Метод для создания пациента с болезнью
        public Patient CreatePatient(string surname, string name, string patronymic,
                                   string phone, string email, DateTime dateOfBirth,
                                   string symptom, string symptomDescription)
        {
            // Создаем болезнь
            var disease = new Disease(
                disId: 0, // ID сгенерируется в репозитории
                title: symptom,
                symptom: symptomDescription
            );

            // Создаем пациента
            return new Patient(
                personId: 0,
                name: name,
                surname: surname,
                patronymic: patronymic,
                phone: phone,
                email: email,
                dateOfBirth: dateOfBirth,
                diseaseId: 0, // Свяжется с болезнью
                disease: disease
            );
        }
    }  
}
