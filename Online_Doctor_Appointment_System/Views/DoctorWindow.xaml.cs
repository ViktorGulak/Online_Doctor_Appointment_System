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
    /// Логика взаимодействия для DoctorWindow.xaml
    /// </summary>
    public partial class DoctorWindow : Window
    {

        private readonly DoctorService _doctorService;
        private readonly SpecializationService _specializationService;
        private Doctor _selectedDoctor;
        private List<Specialization> _allSpecializations; // Список всех специальностей
        public DoctorWindow()
        {
            InitializeComponent();

            // Инициализация сервисов
            XmlDoctorRepository doctorRepo = new XmlDoctorRepository();
            _doctorService = new DoctorService(doctorRepo);

            SpecializationRepository specRepo = new SpecializationRepository();
            _specializationService = new SpecializationService(specRepo);

            // Загружаем данные
            LoadSpecializations(); // Загружаем специальности из файла
            LoadDoctors(); // Загружаем врачей
        }

        private void LoadSpecializations()
        {
            try
            {
                // Загружаем все специальности из файла
                _allSpecializations = _specializationService.GetAllSpecializations();

                // Заполняем ComboBox
                SpecTitleCB.ItemsSource = _allSpecializations;
                SpecTitleCB.DisplayMemberPath = "SpecializationTitle";
                SpecTitleCB.SelectedValuePath = "SpecializationId";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки специальностей: {ex.Message}");
            }
        }

        private void LoadDoctors()
        {
            // Прямо используем объекты Doctor в DataGrid
            List<Doctor> doctors = _doctorService.GetAllDoctors();
            PersonsDG.ItemsSource = doctors;
        }

        private void ClearTextFields()
        {
            LicenseNumberTB.Clear();
            ExperienceTB.Clear();
            DocSurnameTB.Clear();
            DocNameTB.Clear();
            DocPatronymicTB.Clear();
            SpecDescriptionTB.Clear();
            SpecTitleCB.SelectedIndex = -1;
        }

        private void SpecTitleCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // При выборе специальности показываем её описание
            if (SpecTitleCB.SelectedItem is Specialization selectedSpec)
            {
                SpecDescriptionTB.Text = selectedSpec.SpecializationDescription;
            }
            else
            {
                SpecDescriptionTB.Text = "";
            }
        }

        private void PersonsDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PersonsDG.SelectedItem is Doctor selectedDoctor)
            {
                _selectedDoctor = selectedDoctor;

                // Заполняем поля формы
                DocSurnameTB.Text = selectedDoctor.Surname;
                DocNameTB.Text = selectedDoctor.Name;
                DocPatronymicTB.Text = selectedDoctor.Patronymic;
                LicenseNumberTB.Text = selectedDoctor.LicenseNumber;
                ExperienceTB.Text = selectedDoctor.Experience.ToString();

                // Устанавливаем специальность
                if (selectedDoctor.DoctorSpecialization != null)
                {
                    SpecTitleCB.Text = selectedDoctor.DoctorSpecialization.SpecializationTitle;
                    SpecDescriptionTB.Text = selectedDoctor.DoctorSpecialization.SpecializationDescription;
                }
            }
        }

        private void AddDoctorBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация полей
                if (string.IsNullOrWhiteSpace(DocSurnameTB.Text) ||
                    string.IsNullOrWhiteSpace(DocNameTB.Text) ||
                    string.IsNullOrWhiteSpace(LicenseNumberTB.Text) ||
                    string.IsNullOrWhiteSpace(ExperienceTB.Text) ||
                    SpecTitleCB.SelectedItem == null)
                {
                    MessageBox.Show("Заполните все обязательные поля!");
                    return;
                }

                if (!int.TryParse(ExperienceTB.Text, out int experience))
                {
                    MessageBox.Show("Стаж должен быть числом!");
                    return;
                }

                // Получаем выбранную специальность
                Specialization selectedSpec = (Specialization)SpecTitleCB.SelectedItem;

                // Создаем врача
                var doctor = new Doctor(
                    personId: 0, // ID сгенерируется в репозитории
                    name: DocNameTB.Text,
                    surname: DocSurnameTB.Text,
                    patronymic: DocPatronymicTB.Text,
                    specId: selectedSpec.SpecializationId,
                    spec: selectedSpec, // Полный объект специальности
                    licNum: LicenseNumberTB.Text,
                    exp: experience
                );

                // Добавляем врача
                _doctorService.AddDoctor(doctor);

                // Обновляем список
                LoadDoctors();
                ClearTextFields();
                MessageBox.Show("Врач успешно добавлен!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void ClearDoctorBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearTextFields();
        }

        private void UpdateDoctorBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedDoctor == null)
            {
                MessageBox.Show("Выберите врача для редактирования!");
                return;
            }

            try
            {
                if (!int.TryParse(ExperienceTB.Text, out int experience))
                {
                    MessageBox.Show("Стаж должен быть числом!");
                    return;
                }

                // Получаем выбранную специальность
                Specialization selectedSpec = (Specialization)SpecTitleCB.SelectedItem;

                // Обновляем данные врача
                _selectedDoctor.Name = DocNameTB.Text;
                _selectedDoctor.Surname = DocSurnameTB.Text;
                _selectedDoctor.Patronymic = DocPatronymicTB.Text;
                _selectedDoctor.LicenseNumber = LicenseNumberTB.Text;
                _selectedDoctor.Experience = experience;
                _selectedDoctor.SpecializationId = selectedSpec.SpecializationId;
                _selectedDoctor.DoctorSpecialization = selectedSpec;

                // Сохраняем изменения
                _doctorService.UpdateDoctor(_selectedDoctor);

                // Обновляем DataGrid
                PersonsDG.ItemsSource = null;
                PersonsDG.ItemsSource = _doctorService.GetAllDoctors();
                ClearTextFields();
                MessageBox.Show("Данные врача обновлены!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void DeleteDoctorBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedDoctor == null)
            {
                MessageBox.Show("Выберите врача для удаления!");
                return;
            }

            var result = MessageBox.Show($"Удалить врача {_selectedDoctor.FullName}?",
                                       "Подтверждение удаления",
                                       MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _doctorService.DeleteDoctor(_selectedDoctor.PersonId);
                    LoadDoctors();
                    ClearTextFields();
                    MessageBox.Show("Врач удален!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }

        private void DocSearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Получаем текст из поля поиска
            string searchTerm = DocSearchTB.Text.Trim();

            // Если поле пустое - показываем всех врачей
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                LoadDoctors(); // Ваш существующий метод загрузки
                return;
            }

            try
            {
                // Выполняем поиск через сервис
                List<Doctor> searchResults = _doctorService.SearchDoctors(searchTerm);

                // Обновляем DataGrid результатами поиска
                PersonsDG.ItemsSource = searchResults;
            }
            catch (Exception ex)
            {
                // Если ошибка - показываем сообщение и загружаем всех врачей
                MessageBox.Show($"Ошибка поиска: {ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
                LoadDoctors();
            }
        }
    }
}
