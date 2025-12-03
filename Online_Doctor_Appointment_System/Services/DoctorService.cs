using Online_Doctor_Appointment_System.Models;
using Online_Doctor_Appointment_System.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online_Doctor_Appointment_System.Services
{
    public class DoctorService
    {
        private readonly IRepository<Doctor> _doctorRepository;

        public DoctorService(IRepository<Doctor> doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        public List<Doctor> GetAllDoctors() => _doctorRepository.GetAll();

        public Doctor GetDoctorById(long id) => _doctorRepository.GetById(id);

        public void AddDoctor(Doctor doctor) => _doctorRepository.Add(doctor);

        public void UpdateDoctor(Doctor doctor) => _doctorRepository.Update(doctor);

        public void DeleteDoctor(long id) => _doctorRepository.Delete(id);

        // Оптимизированный поиск (быстрее)
        public List<Doctor> SearchDoctors(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAllDoctors();

            List<Doctor> allDoctors = GetAllDoctors();
            searchTerm = searchTerm.ToLowerInvariant();

            return allDoctors.Where(doctor =>
            {
                // Поиск по ФИО
                if (doctor.Name.ToLowerInvariant().Contains(searchTerm) ||
                    doctor.Surname.ToLowerInvariant().Contains(searchTerm) ||
                    doctor.Patronymic.ToLowerInvariant().Contains(searchTerm) ||
                    doctor.FullName.ToLowerInvariant().Contains(searchTerm))
                    return true;

                // Поиск по номеру лицензии
                if (doctor.LicenseNumber.ToLowerInvariant().Contains(searchTerm))
                    return true;

                // Поиск по стажу
                if (doctor.Experience.ToString().Contains(searchTerm))
                    return true;

                // Поиск по специальности
                if (doctor.DoctorSpecialization != null)
                {
                    if (doctor.DoctorSpecialization.SpecializationTitle
                        .ToLowerInvariant().Contains(searchTerm) ||
                        doctor.DoctorSpecialization.SpecializationDescription
                        .ToLowerInvariant().Contains(searchTerm))
                        return true;
                }

                return false;
            }).ToList();
        }
    }
}
