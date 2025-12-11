using Online_Doctor_Appointment_System.Models;
using Online_Doctor_Appointment_System.Models.AnalyticsModels;
using Online_Doctor_Appointment_System.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Online_Doctor_Appointment_System.Services.AnalyticsServices
{
    public class DoctorEfficiencyAnalyzer
    {
        private readonly IRepository<Appointment> _appointmentRepository;
        private readonly DoctorService _doctorService;

        public DoctorEfficiencyAnalyzer(IRepository<Appointment> appointmentRepository,
                                       DoctorService doctorService)
        {
            _appointmentRepository = appointmentRepository;
            _doctorService = doctorService;
        }

        // Рассчитываем эффективность по формуле:
        // Эффективность = w1*Рейтинг + w2*Стаж + w3*Процент успешных записей - w4*Среднее время ожидания
        public List<DoctorEfficiency> CalculateEfficiency(
         double ratingWeight = 0.3,
         double experienceWeight = 0.25,
         double successRateWeight = 0.35,
         double waitTimeWeight = 0.1)
        {
            List<Doctor> doctors = _doctorService.GetAllDoctors();
            List<Appointment> appointments = _appointmentRepository.GetAll();

            List<DoctorEfficiency> efficiencies = new List<DoctorEfficiency>();

            foreach (var doctor in doctors)
            {
                List<Appointment> doctorAppointments = appointments
                    .Where(a => a.DoctorId == doctor.PersonId)
                    .ToList();

                if (doctorAppointments.Count == 0)
                    continue;

                // 1. Рассчитываем процент выполненых записей
                int totalAppointments = doctorAppointments.Count;
                int successfulAppointments = doctorAppointments
                    .Count(a => a.Status == AppointmentStatus.COMPLETED);
                double successRate = totalAppointments > 0 ?
                    (double)successfulAppointments / totalAppointments * 100 : 0;

                // 2. Средняя продолжительность приема
                // Используем DefaultIfEmpty, чтобы избежать ошибки при пустой коллекции
                double averageDuration = doctorAppointments
                    .Where(a => a.Status == AppointmentStatus.COMPLETED)
                    .Select(a => a.EndTime.TotalMinutes - a.StartTime.TotalMinutes)
                    .DefaultIfEmpty(30) // Значение по умолчанию - 30 минут
                    .Average();

                // 3. Рассчитываем эффективность
                // Исправляем формулу: не вычитаем waitTimeWeight, а умножаем на коэффициент времени
                double timeEfficiency = 100 - Math.Min(averageDuration, 60) * waitTimeWeight;

                double efficiencyScore =
                    (doctor.Experience * experienceWeight) +
                    (successRate * successRateWeight) +
                    timeEfficiency;

                efficiencies.Add(new DoctorEfficiency
                {
                    Doctor = doctor,
                    EfficiencyScore = Math.Round(efficiencyScore, 2),
                    SuccessRate = Math.Round(successRate, 2),
                    AverageDuration = Math.Round(averageDuration, 2),
                    TotalAppointments = totalAppointments,
                    SuccessfulAppointments = successfulAppointments
                });
            }

            return efficiencies.OrderByDescending(e => e.EfficiencyScore).ToList();
        }
    }
}
