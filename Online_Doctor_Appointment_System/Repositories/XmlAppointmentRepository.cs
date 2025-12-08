using Online_Doctor_Appointment_System.Models;
using Online_Doctor_Appointment_System.Services;
using Online_Doctor_Appointment_System.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace Online_Doctor_Appointment_System.Repositories
{
    public class XmlAppointmentRepository : IRepository<Appointment>
    {
        private readonly string _filePath;
        private List<Appointment> _appointments;

        public XmlAppointmentRepository()
        {
            _filePath = @"C:\Users\User\source\repos\Online_Doctor_Appointment_System\Online_Doctor_Appointment_System\Store\Appointments.xml";
            _appointments = LoadDataManually();
        }

        private List<Appointment> LoadDataManually()
        {
            var appointments = new List<Appointment>();

            try
            {
                if (!File.Exists(_filePath))
                {
                    MessageBox.Show("Файл записей не найден");
                    return appointments;
                }

                var xmlDoc = new XmlDocument();
                xmlDoc.Load(_filePath);

                // Загружаем HomeVisitAppointment
                var homeVisitNodes = xmlDoc.SelectNodes("//HomeVisitAppointment");
                foreach (XmlNode node in homeVisitNodes)
                {
                    var appointment = new HomeVisitAppointment
                    {
                        AppointmentId = long.Parse(node.SelectSingleNode("AppointmentId")?.InnerText ?? "0"),
                        PatientId = long.Parse(node.SelectSingleNode("PatientId")?.InnerText ?? "0"),
                        DoctorId = long.Parse(node.SelectSingleNode("DoctorId")?.InnerText ?? "0"),
                        AppointmentDate = DateTime.Parse(node.SelectSingleNode("AppointmentDate")?.InnerText ?? DateTime.Now.ToString()),
                        StartTime = TimeSpan.Parse(node.SelectSingleNode("StartTime")?.InnerText ?? "09:00:00"),
                        EndTime = TimeSpan.Parse(node.SelectSingleNode("EndTime")?.InnerText ?? "10:00:00"),
                        Postscript = node.SelectSingleNode("Postscript")?.InnerText ?? "",
                        Status = (AppointmentStatus)Enum.Parse(typeof(AppointmentStatus),
                            node.SelectSingleNode("Status")?.InnerText ?? "SCHEDULED"),
                        City = node.SelectSingleNode("City")?.InnerText ?? "",
                        StreetTitle = node.SelectSingleNode("StreetTitle")?.InnerText ?? "",
                        HouseNumber = node.SelectSingleNode("HouseNumber")?.InnerText ?? "",
                        AppartmentNumber = node.SelectSingleNode("AppartmentNumber")?.InnerText ?? ""
                    };

                    appointments.Add(appointment);
                }

                // Загружаем InPersonAppointment
                var inPersonNodes = xmlDoc.SelectNodes("//InPersonAppointment");
                foreach (XmlNode node in inPersonNodes)
                {
                    var appointment = new InPersonAppointment
                    {
                        AppointmentId = long.Parse(node.SelectSingleNode("AppointmentId")?.InnerText ?? "0"),
                        PatientId = long.Parse(node.SelectSingleNode("PatientId")?.InnerText ?? "0"),
                        DoctorId = long.Parse(node.SelectSingleNode("DoctorId")?.InnerText ?? "0"),
                        AppointmentDate = DateTime.Parse(node.SelectSingleNode("AppointmentDate")?.InnerText ?? DateTime.Now.ToString()),
                        StartTime = TimeSpan.Parse(node.SelectSingleNode("StartTime")?.InnerText ?? "09:00:00"),
                        EndTime = TimeSpan.Parse(node.SelectSingleNode("EndTime")?.InnerText ?? "10:00:00"),
                        Postscript = node.SelectSingleNode("Postscript")?.InnerText ?? "",
                        Status = (AppointmentStatus)Enum.Parse(typeof(AppointmentStatus),
                            node.SelectSingleNode("Status")?.InnerText ?? "SCHEDULED"),
                        HospitalTitle = node.SelectSingleNode("HospitalTitle")?.InnerText ?? "",
                        CabinetNumber = node.SelectSingleNode("CabinetNumber")?.InnerText ?? ""
                    };

                    appointments.Add(appointment);
                }

                // Загружаем OnlineAppointment
                var onlineNodes = xmlDoc.SelectNodes("//OnlineAppointment");
                foreach (XmlNode node in onlineNodes)
                {
                    var appointment = new OnlineAppointment
                    {
                        AppointmentId = long.Parse(node.SelectSingleNode("AppointmentId")?.InnerText ?? "0"),
                        PatientId = long.Parse(node.SelectSingleNode("PatientId")?.InnerText ?? "0"),
                        DoctorId = long.Parse(node.SelectSingleNode("DoctorId")?.InnerText ?? "0"),
                        AppointmentDate = DateTime.Parse(node.SelectSingleNode("AppointmentDate")?.InnerText ?? DateTime.Now.ToString()),
                        StartTime = TimeSpan.Parse(node.SelectSingleNode("StartTime")?.InnerText ?? "09:00:00"),
                        EndTime = TimeSpan.Parse(node.SelectSingleNode("EndTime")?.InnerText ?? "10:00:00"),
                        Postscript = node.SelectSingleNode("Postscript")?.InnerText ?? "",
                        Status = (AppointmentStatus)Enum.Parse(typeof(AppointmentStatus),
                            node.SelectSingleNode("Status")?.InnerText ?? "SCHEDULED"),
                        Link = node.SelectSingleNode("Link")?.InnerText ?? "",
                        ChatCode = node.SelectSingleNode("ChatCode")?.InnerText ?? ""
                    };

                    appointments.Add(appointment);
                }
                return appointments;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка ручной загрузки: {ex.Message}");
                return new List<Appointment>();
            }
        }

        // Метод для сохранения
        private void SaveDataManually()
        {
            try
            {
                var xmlDoc = new XmlDocument();
                var root = xmlDoc.CreateElement("ArrayOfAppointment");
                xmlDoc.AppendChild(root);

                foreach (var appointment in _appointments)
                {
                    XmlElement appointmentElement;

                    if (appointment is HomeVisitAppointment homeVisit)
                    {
                        appointmentElement = xmlDoc.CreateElement("HomeVisitAppointment");
                        AddChild(xmlDoc, appointmentElement, "City", homeVisit.City);
                        AddChild(xmlDoc, appointmentElement, "StreetTitle", homeVisit.StreetTitle);
                        AddChild(xmlDoc, appointmentElement, "HouseNumber", homeVisit.HouseNumber);
                        AddChild(xmlDoc, appointmentElement, "AppartmentNumber", homeVisit.AppartmentNumber);
                    }
                    else if (appointment is InPersonAppointment inPerson)
                    {
                        appointmentElement = xmlDoc.CreateElement("InPersonAppointment");
                        AddChild(xmlDoc, appointmentElement, "HospitalTitle", inPerson.HospitalTitle);
                        AddChild(xmlDoc, appointmentElement, "CabinetNumber", inPerson.CabinetNumber);
                    }
                    else if (appointment is OnlineAppointment online)
                    {
                        appointmentElement = xmlDoc.CreateElement("OnlineAppointment");
                        AddChild(xmlDoc, appointmentElement, "Link", online.Link);
                        AddChild(xmlDoc, appointmentElement, "ChatCode", online.ChatCode);
                    }
                    else
                    {
                        continue;
                    }

                    // Общие поля
                    AddChild(xmlDoc, appointmentElement, "AppointmentId", appointment.AppointmentId.ToString());
                    AddChild(xmlDoc, appointmentElement, "PatientId", appointment.PatientId.ToString());
                    AddChild(xmlDoc, appointmentElement, "DoctorId", appointment.DoctorId.ToString());
                    AddChild(xmlDoc, appointmentElement, "AppointmentDate", appointment.AppointmentDate.ToString("yyyy-MM-ddTHH:mm:ss"));
                    AddChild(xmlDoc, appointmentElement, "StartTime", appointment.StartTime.ToString());
                    AddChild(xmlDoc, appointmentElement, "EndTime", appointment.EndTime.ToString());
                    AddChild(xmlDoc, appointmentElement, "Postscript", appointment.Postscript);
                    AddChild(xmlDoc, appointmentElement, "Status", appointment.Status.ToString());

                    root.AppendChild(appointmentElement);
                }

                xmlDoc.Save(_filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void AddChild(XmlDocument xmlDoc, XmlElement parent, string name, string value)
        {
            var child = xmlDoc.CreateElement(name);
            child.InnerText = value ?? "";
            parent.AppendChild(child);
        }

        public List<Appointment> GetAll() => new List<Appointment>(_appointments);

        public Appointment GetById(long id) => _appointments.FirstOrDefault(a => a.AppointmentId == id);

        public void Add(Appointment appointment)
        {
            if (appointment.AppointmentId == 0)
            {
                appointment.AppointmentId = _appointments.Count > 0 ?
                    _appointments.Max(a => a.AppointmentId) + 1 : 1;
            }

            _appointments.Add(appointment);
            SaveDataManually();
        }

        public void Update(Appointment appointment)
        {
            var existing = GetById(appointment.AppointmentId);
            if (existing != null)
            {
                var index = _appointments.IndexOf(existing);
                _appointments[index] = appointment;
                SaveDataManually();
            }
        }

        public void Delete(long id)
        {
            var appointment = GetById(id);
            if (appointment != null)
            {
                _appointments.Remove(appointment);
                SaveDataManually();
            }
        }

        public bool Exists(long id) => _appointments.Any(a => a.AppointmentId == id);
    }
}
