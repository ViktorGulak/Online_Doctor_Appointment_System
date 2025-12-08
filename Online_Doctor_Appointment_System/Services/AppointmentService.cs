using Online_Doctor_Appointment_System.Models;
using Online_Doctor_Appointment_System.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online_Doctor_Appointment_System.Services
{
    public class AppointmentService
    {
        private readonly IRepository<Appointment> _appointmentRepository;
        private readonly DoctorService _doctorService;
        private readonly PatientService _patientService;

        public AppointmentService(
            IRepository<Appointment> appointmentRepository,
            DoctorService doctorService,
            PatientService patientService)
        {
            _appointmentRepository = appointmentRepository;
            _doctorService = doctorService;
            _patientService = patientService;
        }

        public List<Appointment> GetAllAppointments()
        {
            var appointments = _appointmentRepository.GetAll();
            LinkAppointments(appointments); // Связываем с врачами и пациентами
            return appointments;
        }

        public Appointment GetAppointmentById(long id)
        {
            var appointment = _appointmentRepository.GetById(id);
            if (appointment != null)
            {
                LinkAppointment(appointment);
            }
            return appointment;
        }

        private void LinkAppointments(List<Appointment> appointments)
        {
            foreach (var appointment in appointments)
            {
                LinkAppointment(appointment);
            }
        }

        private void LinkAppointment(Appointment appointment)
        {
            // Находим врача
            if (appointment.DoctorId > 0 && appointment.ActingDoctor == null)
            {
                appointment.ActingDoctor = _doctorService.GetDoctorById(appointment.DoctorId);
            }

            // Находим пациента
            if (appointment.PatientId > 0 && appointment.RecordedPatient == null)
            {
                appointment.RecordedPatient = _patientService.GetPatientById(appointment.PatientId);
            }
        }

        public void AddAppointment(Appointment appointment) => _appointmentRepository.Add(appointment);
        public void UpdateAppointment(Appointment appointment) => _appointmentRepository.Update(appointment);
        public void DeleteAppointment(long id) => _appointmentRepository.Delete(id);
    }
}
