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
            LinkAppointments(appointments);
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

        // Создание новой записи в зависимости от типа
        public Appointment CreateAppointment(
            AppointmentType type,
            long patientId,
            long doctorId,
            DateTime appointmentDate,
            TimeSpan startTime,
            TimeSpan endTime,
            string postscript,
            AppointmentStatus status,
            // Дополнительные параметры для разных типов
            string city = null,
            string streetTitle = null,
            string houseNumber = null,
            string appartmentNumber = null,
            string hospitalTitle = null,
            string cabinetNumber = null,
            string link = null,
            string chatCode = null)
        {
            // Получаем объекты врача и пациента
            var doctor = _doctorService.GetDoctorById(doctorId);
            var patient = _patientService.GetPatientById(patientId);
            Appointment appointment;
            if(type == AppointmentType.HomeVisit)
            {
                appointment = new HomeVisitAppointment(
                    appointmentId: 0,
                    patientId: patientId,
                    patient: patient,
                    doctorId: doctorId,
                    doctor: doctor,
                    appointmentDateTime: appointmentDate,
                    startTime: startTime,
                    endTime: endTime,
                    postscript: postscript,
                    status: status,
                    city: city,
                    streetTitle: streetTitle,
                    houseNumber: houseNumber,
                    appartmentNumber: appartmentNumber);
            }
            else if(type == AppointmentType.InPerson)
            {
                appointment = new InPersonAppointment(
                    appointmentId: 0,
                    patientId: patientId,
                    patient: patient,
                    doctorId: doctorId,
                    doctor: doctor,
                    appointmentDateTime: appointmentDate,
                    startTime: startTime,
                    endTime: endTime,
                    postscript: postscript,
                    status: status,
                    hospitalTitle: hospitalTitle,
                    cabinetNumber: cabinetNumber);
            }
            else if (type == AppointmentType.Online)
            {
                appointment = new OnlineAppointment(
                    appointmentId: 0,
                    patientId: patientId,
                    patient: patient,
                    doctorId: doctorId,
                    doctor: doctor,
                    appointmentDateTime: appointmentDate,
                    startTime: startTime,
                    endTime: endTime,
                    postscript: postscript,
                    status: status,
                    link: link,
                    chatCode: chatCode);
            }
            else
            {
                throw new ArgumentException("Неизвестный тип записи");
            }
            return appointment;
        }

        public void AddAppointment(Appointment appointment)
        {
            // Проверка на пересечение времени
            if (!IsDoctorAvailable(appointment.DoctorId, appointment.AppointmentDate, appointment.StartTime, appointment.EndTime))
            {
                throw new InvalidOperationException("Врач уже занят в это время");
            }

            _appointmentRepository.Add(appointment);
        }

        public void UpdateAppointment(Appointment appointment)
        {
            var existing = GetAppointmentById(appointment.AppointmentId);
            if (existing == null)
                throw new InvalidOperationException("Запись не найдена");

            // Проверка на пересечение времени (исключая текущую запись)
            if (!IsDoctorAvailable(appointment.DoctorId, appointment.AppointmentDate,
                appointment.StartTime, appointment.EndTime, appointment.AppointmentId))
            {
                throw new InvalidOperationException("Врач уже занят в это время");
            }

            _appointmentRepository.Update(appointment);
        }

        public void DeleteAppointment(long id) => _appointmentRepository.Delete(id);

        private void LinkAppointments(List<Appointment> appointments)
        {
            foreach (var appointment in appointments)
            {
                LinkAppointment(appointment);
            }
        }

        private void LinkAppointment(Appointment appointment)
        {
            if (appointment.DoctorId > 0 && appointment.ActingDoctor == null)
            {
                appointment.ActingDoctor = _doctorService.GetDoctorById(appointment.DoctorId);
            }

            if (appointment.PatientId > 0 && appointment.RecordedPatient == null)
            {
                appointment.RecordedPatient = _patientService.GetPatientById(appointment.PatientId);
            }
        }

        // Проверка доступности врача
        public bool IsDoctorAvailable(long doctorId, DateTime date, TimeSpan startTime, TimeSpan endTime, long? excludeAppointmentId = null)
        {
            var appointments = _appointmentRepository.GetAll()
                .Where(a => a.DoctorId == doctorId &&
                           a.Status != AppointmentStatus.CANCELLED &&
                           a.AppointmentId != excludeAppointmentId)
                .ToList();

            // Создаем тестовую запись для проверки пересечений
            var testAppointment = new HomeVisitAppointment
            {
                DoctorId = doctorId,
                AppointmentDate = date,
                StartTime = startTime,
                EndTime = endTime
            };

            return !appointments.Any(a => testAppointment.TimeOverlapsWith(a));
        }

        // Поиск записей
        public List<Appointment> SearchAppointments(string searchTerm)
        {
            var allAppointments = GetAllAppointments();

            if (string.IsNullOrWhiteSpace(searchTerm))
                return allAppointments;

            var term = searchTerm.ToLowerInvariant();

            return allAppointments.Where(a =>
            {
                if (a.ActingDoctor != null && (
                    a.ActingDoctor.Name.ToLowerInvariant().Contains(term) ||
                    a.ActingDoctor.Surname.ToLowerInvariant().Contains(term) ||
                    a.ActingDoctor.FullName.ToLowerInvariant().Contains(term)))
                    return true;

                if (a.RecordedPatient != null && (
                    a.RecordedPatient.Name.ToLowerInvariant().Contains(term) ||
                    a.RecordedPatient.Surname.ToLowerInvariant().Contains(term) ||
                    a.RecordedPatient.FullName.ToLowerInvariant().Contains(term)))
                    return true;

                if (a.Postscript?.ToLowerInvariant().Contains(term) == true)
                    return true;

                if (a.AdditionalInfo?.ToLowerInvariant().Contains(term) == true)
                    return true;

                return false;
            }).ToList();
        }

        public List<Appointment> FilterAppointmentByStatus(AppointmentStatus status)
        {
            return _appointmentRepository.GetAll().Where(a => a.Status == status).ToList();   
        }
    }
}
