using Online_Doctor_Appointment_System.Models;
using Online_Doctor_Appointment_System.Repositories;
using Online_Doctor_Appointment_System.Services;
using Online_Doctor_Appointment_System.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
            XmlAppointmentRepository appointmentRepo = new XmlAppointmentRepository();
            XmlDoctorRepository doctorRepo = new XmlDoctorRepository();
            XmlPatientRepository patientRepo = new XmlPatientRepository();

            _doctorService = new DoctorService(doctorRepo);
            _patientService = new PatientService(patientRepo);
            _appointmentService = new AppointmentService(appointmentRepo, _doctorService, _patientService);

            // Загрузка данных
            LoadData();
            //TestSimpleLoad();
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
                InitializeStatusComboBoxes();
                InitializeTimeFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void LoadDoctors()
        {
            List<Doctor> doctors = _doctorService.GetAllDoctors();
            ProfDocCB.ItemsSource = doctors;
            ProfDocCB.DisplayMemberPath = "DoctorProfession";
            ProfDocCB.SelectedValuePath = "PersonId";
        }

        private void LoadPatients()
        {
            List<Patient> patients = _patientService.GetAllPatients();
            PatFullNameCB.ItemsSource = patients;
            PatFullNameCB.DisplayMemberPath = "FullName";
            PatFullNameCB.SelectedValuePath = "PersonId";
        }

        private void LoadAppointments()
        {
            _allAppointments = _appointmentService.GetAllAppointments();
            RecordsDG.ItemsSource = _allAppointments;
        }

        private void InitializeTimeFields()
        {
            StartTimeTB.Text = "09:00";
            EndTimeTB.Text = "10:00";
        }
        private void InitializeStatusComboBoxes()
        {
            // Для формы
            StatusCB.ItemsSource = Enum.GetValues(typeof(AppointmentStatus))
                .Cast<AppointmentStatus>()
                .Select(s => new { Value = s, Display = GetStatusDisplayName(s) })
                .ToList();
            StatusCB.DisplayMemberPath = "Display";
            StatusCB.SelectedValuePath = "Value";

            // Для фильтра
            StatusFilterCB.ItemsSource = new List<object>
        {
            new { Value = "ALL", Display = "Все" },
            new { Value = "SCHEDULED", Display = "Запланированные" },
            new { Value = "COMPLETED", Display = "Выполненные" },
            new { Value = "CANCELLED", Display = "Отменённые" },
            new { Value = "NO_SHOW", Display = "Не явки" }
        };
            StatusFilterCB.DisplayMemberPath = "Display";
            StatusFilterCB.SelectedValuePath = "Value";
            StatusFilterCB.SelectedIndex = 0;
        }

        private string GetStatusDisplayName(AppointmentStatus status)
        {
            if (status == AppointmentStatus.SCHEDULED)
            {
                return "Запланирован";
            }
            else if (status == AppointmentStatus.COMPLETED)
            {
                return "Выполнен";
            }
            else if (status == AppointmentStatus.CANCELLED)
            {
                return "Отменён";
            }
            else if (status == AppointmentStatus.NO_SHOW)
            {
                return "Не явка";
            }
            else return "Неопределено";
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
            ClearTextFields();
        }

        private void ClearTextFields()
        {
            ProfDocCB.SelectedIndex = -1;
            PatFullNameCB.SelectedIndex = -1;
            DiagnosisTB.Clear();
            StatusCB.SelectedIndex = 0;
            DateAdmissionDP.SelectedDate = null;
            StartTimeTB.Text = "09:00";
            EndTimeTB.Text = "10:00";
            PostscriptTB.Clear();

            PatientCityTB.Clear();
            PatientStreetTB.Clear();
            HouseNumberTB.Clear();
            AppartmentNumberTB.Clear();

            HospitalTB.Clear();
            CabinetTB.Clear();

            LinkTB.Clear();
            ConnectionCodeTB.Clear();

            TypeOfReceptionCB.SelectedIndex = 0;
            _selectedAppointment = null;
            RecordsDG.SelectedItem = null;

            RecordSearchTB.Clear();
            StatusFilterCB.SelectedIndex = 0;
        }
        private bool ValidateForm()
        {
            if (ProfDocCB.SelectedItem == null)
            {
                MessageBox.Show("Выберите врача!");
                return false;
            }

            if (PatFullNameCB.SelectedItem == null)
            {
                MessageBox.Show("Выберите пациента!");
                return false;
            }

            if (DateAdmissionDP.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату приема!");
                return false;
            }

            if (string.IsNullOrWhiteSpace(StartTimeTB.Text) ||
                string.IsNullOrWhiteSpace(EndTimeTB.Text))
            {
                MessageBox.Show("Укажите время начала и окончания приема!");
                return false;
            }

            if (!TimeSpan.TryParse(StartTimeTB.Text, out TimeSpan startTime) ||
                !TimeSpan.TryParse(EndTimeTB.Text, out TimeSpan endTime))
            {
                MessageBox.Show("Некорректный формат времени! Используйте HH:mm");
                return false;
            }

            if (startTime >= endTime)
            {
                MessageBox.Show("Время начала должно быть раньше времени окончания!");
                return false;
            }

            // Проверки для конкретных типов
            switch (TypeOfReceptionCB.SelectedIndex)
            {
                case 0: // Приём в поликлинике
                    if (string.IsNullOrWhiteSpace(HospitalTB.Text))
                    {
                        MessageBox.Show("Укажите название больницы!");
                        return false;
                    }
                    break;

                case 1: // Выезд на дом
                    if (string.IsNullOrWhiteSpace(PatientCityTB.Text) ||
                        string.IsNullOrWhiteSpace(PatientStreetTB.Text) ||
                        string.IsNullOrWhiteSpace(HouseNumberTB.Text))
                    {
                        MessageBox.Show("Заполните все поля адреса!");
                        return false;
                    }
                    break;

                case 2: // Онлайн
                    if (string.IsNullOrWhiteSpace(LinkTB.Text))
                    {
                        MessageBox.Show("Укажите ссылку на консультацию!");
                        return false;
                    }
                    break;
            }

            return true;
        }

        private AppointmentType GetSelectedAppointmentType()
        {
            AppointmentType appType;
            switch (TypeOfReceptionCB.SelectedIndex)
            {
                case 0:
                    appType = AppointmentType.InPerson;
                    break;
                case 1:
                    appType = AppointmentType.HomeVisit;
                    break;
                case 2:
                    appType = AppointmentType.Online;
                    break;
                default:
                    appType = AppointmentType.HomeVisit;
                    break;
            }
            return appType;
            
        }

        private TimeSpan ParseTime(string timeText)
        {
            if (TimeSpan.TryParse(timeText, out TimeSpan time))
                return time;

            if (timeText.Contains(":"))
            {
                var parts = timeText.Split(':');
                if (parts.Length >= 2 &&
                    int.TryParse(parts[0], out int hours) &&
                    int.TryParse(parts[1], out int minutes))
                {
                    return new TimeSpan(hours, minutes, 0);
                }
            }

            return new TimeSpan(9, 0, 0); // По умолчанию 9:00
        }

        private Appointment CreateAppointmentFromForm(AppointmentType type)
        {
            // Получаем ID врача и пациента
            long doctorId = ProfDocCB.SelectedValue is long docId ? docId : 0;
            long patientId = PatFullNameCB.SelectedValue is long patId ? patId : 0;

            // Парсим время
            TimeSpan startTime = ParseTime(StartTimeTB.Text);
            TimeSpan endTime = ParseTime(EndTimeTB.Text);

            // Получаем статус
            var status = StatusCB.SelectedValue is AppointmentStatus stat ? stat : AppointmentStatus.SCHEDULED;
            if(type == AppointmentType.HomeVisit)
            {
                return _appointmentService.CreateAppointment(
                    type: AppointmentType.HomeVisit,
                    patientId: patientId,
                    doctorId: doctorId,
                    appointmentDate: DateAdmissionDP.SelectedDate ?? DateTime.Now,
                    startTime: startTime,
                    endTime: endTime,
                    postscript: PostscriptTB.Text,
                    status: status,
                    city: PatientCityTB.Text,
                    streetTitle: PatientStreetTB.Text,
                    houseNumber: HouseNumberTB.Text,
                    appartmentNumber: AppartmentNumberTB.Text);
            }
            else if (type == AppointmentType.InPerson)
            {
                return _appointmentService.CreateAppointment(
                    type: AppointmentType.InPerson,
                    patientId: patientId,
                    doctorId: doctorId,
                    appointmentDate: DateAdmissionDP.SelectedDate ?? DateTime.Now,
                    startTime: startTime,
                    endTime: endTime,
                    postscript: PostscriptTB.Text,
                    status: status,
                    hospitalTitle: HospitalTB.Text,
                    cabinetNumber: CabinetTB.Text);
            }
            else if (type == AppointmentType.Online)
            {
                return _appointmentService.CreateAppointment(
                    type: AppointmentType.Online,
                    patientId: patientId,
                    doctorId: doctorId,
                    appointmentDate: DateAdmissionDP.SelectedDate ?? DateTime.Now,
                    startTime: startTime,
                    endTime: endTime,
                    postscript: PostscriptTB.Text,
                    status: status,
                    link: LinkTB.Text,
                    chatCode: ConnectionCodeTB.Text);
            }
            else
            {
                throw new Exception("Не определённый тип записи."); ;
            }
        }

        private void AddRecordBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateForm())
                    return;

                var appointmentType = GetSelectedAppointmentType();
                var appointment = CreateAppointmentFromForm(appointmentType);

                _appointmentService.AddAppointment(appointment);

                LoadAppointments();
                ClearTextFields();
                MessageBox.Show("Запись успешно добавлена!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении: {ex.Message}");
            }
        }

        private void UpdateRecordBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedAppointment == null)
            {
                MessageBox.Show("Выберите запись для редактирования!");
                return;
            }

            try
            {
                if (!ValidateForm())
                    return;

                var appointmentType = GetSelectedAppointmentType();
                var updatedAppointment = CreateAppointmentFromForm(appointmentType);
                updatedAppointment.AppointmentId = _selectedAppointment.AppointmentId;

                _appointmentService.UpdateAppointment(updatedAppointment);

                LoadAppointments();
                MessageBox.Show("Запись успешно обновлена!");
                ClearTextFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении: {ex.Message}");
            }
        }

        private void DeleteRecordBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedAppointment == null)
            {
                MessageBox.Show("Выберите запись для удаления!");
                return;
            }

            var result = MessageBox.Show($"Удалить запись #{_selectedAppointment.AppointmentId}?",
                                       "Подтверждение удаления",
                                       MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _appointmentService.DeleteAppointment(_selectedAppointment.AppointmentId);
                    LoadAppointments();
                    ClearTextFields();
                    MessageBox.Show("Запись удалена!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                }
            }
        }

        private void RecordSearchTB_SelectionChanged(object sender, RoutedEventArgs e)
        {
            string searchTerm = RecordSearchTB.Text.Trim();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                LoadAppointments();
            }
            else
            {
                var searchResults = _appointmentService.SearchAppointments(searchTerm);
                RecordsDG.ItemsSource = searchResults;
            }
        }

        private void StatusFilterCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_allAppointments == null) return;

            var selected = StatusFilterCB.SelectedValue as string;
            if (selected == "ALL")
            {
                RecordsDG.ItemsSource = _allAppointments;
            }
            else if(selected == "SCHEDULED")
            {
                RecordsDG.ItemsSource = _appointmentService.FilterAppointmentByStatus(AppointmentStatus.SCHEDULED);
            }
            else if (selected == "COMPLETED")
            {
                RecordsDG.ItemsSource = _appointmentService.FilterAppointmentByStatus(AppointmentStatus.COMPLETED);
            }
            else if (selected == "CANCELLED")
            {
                RecordsDG.ItemsSource = _appointmentService.FilterAppointmentByStatus(AppointmentStatus.CANCELLED);
            }
            else if (selected == "NO_SHOW")
            {
                RecordsDG.ItemsSource = _appointmentService.FilterAppointmentByStatus(AppointmentStatus.NO_SHOW);
            }
            else
            {
                RecordsDG.ItemsSource = _allAppointments;
            }
        }

        private void PatFullNameCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Patient selectdPat = PatFullNameCB.SelectedItem as Patient;
            if (selectdPat == null) return;
            DiagnosisTB.Text = selectdPat.DiseaseTitle;
        }
    }
}
