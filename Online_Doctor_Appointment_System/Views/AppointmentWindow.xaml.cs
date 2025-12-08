using Online_Doctor_Appointment_System.Models;
using Online_Doctor_Appointment_System.Repositories;
using Online_Doctor_Appointment_System.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Online_Doctor_Appointment_System.Views
{
    /// <summary>
    /// Логика взаимодействия для AppointmentWindow.xaml
    /// </summary>
    public partial class AppointmentWindow : Window
    {
        private readonly AppointmentService _appointmentService;
        private readonly DoctorService _doctorService;
        private readonly PatientService _patientService;

        private List<Appointment> _allAppointments;
        private Appointment _selectedAppointment;

        public AppointmentWindow()
        {
            InitializeComponent();

            // Инициализация сервисов
            var appointmentRepo = new XmlAppointmentRepository();
            var doctorRepo = new XmlDoctorRepository();
            var patientRepo = new XmlPatientRepository();

            _doctorService = new DoctorService(doctorRepo);
            _patientService = new PatientService(patientRepo);
            _appointmentService = new AppointmentService(appointmentRepo, _doctorService, _patientService);

            // Загрузка данных
            LoadData();
            TestSimpleLoad();
        }


        private void LoadData()
        {
            try
            {
                // Загружаем врачей и пациентов для ComboBox
                LoadDoctors();
                LoadPatients();

                // Загружаем записи
                LoadAppointments();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void TestSimpleLoad()
        {
            try
            {
                // 1. Создаем репозиторий
                var repo = new XmlAppointmentRepository();

                // 2. Получаем все записи
                var appointments = repo.GetAll();

                // 4. Если что-то загрузилось - показываем
                if (appointments.Count > 0)
                {
                    RecordsDG.ItemsSource = appointments;

                    // Тестируем связывание с врачами и пациентами
                    var doctorService = new DoctorService(new XmlDoctorRepository());
                    var patientService = new PatientService(new XmlPatientRepository());

                    foreach (var appointment in appointments)
                    {
                        appointment.ActingDoctor = doctorService.GetDoctorById(appointment.DoctorId);
                        appointment.RecordedPatient = patientService.GetPatientById(appointment.PatientId);
                    }

                    // Обновляем DataGrid
                    RecordsDG.ItemsSource = null;
                    RecordsDG.ItemsSource = appointments;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Тестовая загрузка не удалась: {ex.Message}");
            }
        }
        private void LoadDoctors()
        {
            var doctors = _doctorService.GetAllDoctors();
            ProfDocCB.ItemsSource = doctors;
            ProfDocCB.DisplayMemberPath = "FullName";
            ProfDocCB.SelectedValuePath = "PersonId";
        }

        private void LoadPatients()
        {
            var patients = _patientService.GetAllPatients();
            PatFullNameCB.ItemsSource = patients;
            PatFullNameCB.DisplayMemberPath = "FullName";
            PatFullNameCB.SelectedValuePath = "PersonId";
        }

        private void LoadAppointments()
        {
            _allAppointments = _appointmentService.GetAllAppointments();
            RecordsDG.ItemsSource = _allAppointments;
        }

        // Обработчик выбора в DataGrid
        private void RecordsDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RecordsDG.SelectedItem is Appointment selectedAppointment)
            {
                _selectedAppointment = selectedAppointment;
                FillFormWithAppointment(selectedAppointment);
            }
        }

        private void FillFormWithAppointment(Appointment appointment)
        {
            try
            {
                // Основные данные
                ProfDocCB.SelectedValue = appointment.DoctorId;
                PatFullNameCB.SelectedValue = appointment.PatientId;

                // Диагноз пациента
                if (appointment.RecordedPatient?.PatientDisease != null)
                {
                    DiagnosisTB.Text = appointment.RecordedPatient.PatientDisease.DiseaseTitle;
                }
                else
                {
                    DiagnosisTB.Text = "Не указан";
                }

                // Статус
                StatusCB.SelectedIndex = (int)appointment.Status;

                // Дата
                DateAdmissionDP.SelectedDate = appointment.AppointmentDate;

                // Время
                StartTimeTB.Text = appointment.FormattedStartTime;
                EndTimeTB.Text = appointment.FormattedEndTime;

                // Приписка
                PostscriptTB.Text = appointment.Postscript ?? "";

                // Определяем тип записи
                DetermineAndFillAppointmentType(appointment);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка заполнения формы: {ex.Message}");
            }
        }

        private void DetermineAndFillAppointmentType(Appointment appointment)
        {
            // Сначала скрываем все GroupBox
            PatAddressGB.Visibility = Visibility.Collapsed;
            OnlineDataGB.Visibility = Visibility.Collapsed;
            InPersonGB.Visibility = Visibility.Collapsed;

            if (appointment is HomeVisitAppointment homeVisit)
            {
                TypeOfReceptionCB.SelectedIndex = 1; // Выезд врача на дом
                FillHomeVisitFields(homeVisit);
            }
            else if (appointment is InPersonAppointment inPerson)
            {
                TypeOfReceptionCB.SelectedIndex = 0; // Приём в поликлинике
                FillInPersonFields(inPerson);
            }
            else if (appointment is OnlineAppointment online)
            {
                TypeOfReceptionCB.SelectedIndex = 2; // Онлайн консультация
                FillOnlineFields(online);
            }
        }

        private void FillHomeVisitFields(HomeVisitAppointment homeVisit)
        {
            PatAddressGB.Visibility = Visibility.Visible;

            PatientCityTB.Text = homeVisit.City ?? "";
            PatientStreetTB.Text = homeVisit.StreetTitle ?? "";
            HouseNumberTB.Text = homeVisit.HouseNumber ?? "";
            AppartmentNumberTB.Text = homeVisit.AppartmentNumber ?? "";
        }

        private void FillInPersonFields(InPersonAppointment inPerson)
        {
            InPersonGB.Visibility = Visibility.Visible;

            HospitalTB.Text = inPerson.HospitalTitle ?? "";
            CabinetTB.Text = inPerson.CabinetNumber ?? "";
        }

        private void FillOnlineFields(OnlineAppointment online)
        {
            OnlineDataGB.Visibility = Visibility.Visible;

            LinkTB.Text = online.Link ?? "";
            ConnectionCodeTB.Text = online.ChatCode ?? "";
        }

        // Остальные обработчики событий...
        private void TypeOfReceptionCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (TypeOfReceptionCB.SelectedIndex)
            {
                case 0: // Приём в поликлинике
                    InPersonGB.Visibility = Visibility.Visible;
                    OnlineDataGB.Visibility = Visibility.Collapsed;
                    PatAddressGB.Visibility = Visibility.Collapsed;
                    break;
                case 1: // Выезд врача на дом
                    PatAddressGB.Visibility = Visibility.Visible;
                    InPersonGB.Visibility = Visibility.Collapsed;
                    OnlineDataGB.Visibility = Visibility.Collapsed;
                    break;
                case 2: // Онлайн консультация
                    OnlineDataGB.Visibility = Visibility.Visible;
                    InPersonGB.Visibility = Visibility.Collapsed;
                    PatAddressGB.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        // Кнопка очистки
        private void ClearRecordBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            ProfDocCB.SelectedIndex = -1;
            PatFullNameCB.SelectedIndex = -1;
            DiagnosisTB.Text = "";
            StatusCB.SelectedIndex = 0;
            DateAdmissionDP.SelectedDate = null;
            StartTimeTB.Text = "09:00";
            EndTimeTB.Text = "10:00";
            PostscriptTB.Text = "";

            PatientCityTB.Text = "";
            PatientStreetTB.Text = "";
            HouseNumberTB.Text = "";
            AppartmentNumberTB.Text = "";

            HospitalTB.Text = "";
            CabinetTB.Text = "";

            LinkTB.Text = "";
            ConnectionCodeTB.Text = "";

            TypeOfReceptionCB.SelectedIndex = 0;
            _selectedAppointment = null;
            RecordsDG.SelectedItem = null;
        }
    }
}
