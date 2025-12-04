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
    /// Логика взаимодействия для PatientWindow.xaml
    /// </summary>
    public partial class PatientWindow : Window
    {
        private readonly PatientService _patientService;
        private Patient _selectedPatient;

        public PatientWindow()
        {
            InitializeComponent();

            // Инициализация сервиса
            var patientRepo = new XmlPatientRepository();
            _patientService = new PatientService(patientRepo);

            // Загружаем пациентов
            LoadPatients();
        }

        private void LoadPatients()
        {
            try
            {
                var patients = _patientService.GetAllPatients();
                PaientsDG.ItemsSource = patients;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки пациентов: {ex.Message}");
            }
        }

        private void PaientsDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PaientsDG.SelectedItem is Patient selectedPatient)
            {
                _selectedPatient = selectedPatient;

                // Заполняем поля формы
                PatSurnameTB.Text = selectedPatient.Surname ?? "";
                PatNameTB.Text = selectedPatient.Name ?? "";
                PatPatronymicTB.Text = selectedPatient.Patronymic ?? "";
                PatPhoneTB.Text = selectedPatient.Phone ?? "";
                PatMailTB.Text = selectedPatient.Email ?? "";
                PatDateBirthDP.SelectedDate = selectedPatient.DateOfBirth;

                // Заполняем данные о болезни (ПРОВЕРЯЕМ на null!)
                if (selectedPatient.PatientDisease != null)
                {
                    SymptomTB.Text = selectedPatient.PatientDisease.DiseaseTitle ?? "";
                    SymptomDescriptionTB.Text = selectedPatient.PatientDisease.DiseaseSymptom ?? "";
                }
                else
                {
                    SymptomTB.Text = "";
                    SymptomDescriptionTB.Text = "";
                }
            }
            else
            {
                // Если ничего не выбрано - очищаем форму
                ClearTextFields();
            }
        }

        private void PatSearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchTerm = PatSearchTB.Text.Trim();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                LoadPatients();
            }
            else
            {
                var searchResults = _patientService.SearchPatients(searchTerm);
                PaientsDG.ItemsSource = searchResults;
            }
        }

        private void AddPatientBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация
                if (!ValidatePatientForm())
                    return;

                // Создаем пациента
                var patient = _patientService.CreatePatient(
                    surname: PatSurnameTB.Text,
                    name: PatNameTB.Text,
                    patronymic: PatPatronymicTB.Text,
                    phone: PatPhoneTB.Text,
                    email: PatMailTB.Text,
                    dateOfBirth: PatDateBirthDP.SelectedDate ?? DateTime.Now,
                    symptom: SymptomTB.Text,
                    symptomDescription: SymptomDescriptionTB.Text
                );

                // Добавляем пациента
                _patientService.AddPatient(patient);

                // Обновляем список
                LoadPatients();
                ClearTextFields();
                MessageBox.Show("Пациент успешно добавлен!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void UpdatePatientBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPatient == null)
            {
                MessageBox.Show("Выберите пациента для редактирования!");
                return;
            }

            try
            {
                if (!ValidatePatientForm())
                    return;

                // Обновляем данные пациента
                _selectedPatient.Surname = PatSurnameTB.Text;
                _selectedPatient.Name = PatNameTB.Text;
                _selectedPatient.Patronymic = PatPatronymicTB.Text;
                _selectedPatient.Phone = PatPhoneTB.Text;
                _selectedPatient.Email = PatMailTB.Text;
                _selectedPatient.DateOfBirth = PatDateBirthDP.SelectedDate ?? DateTime.Now;

                // Обновляем или создаем болезнь
                if (_selectedPatient.PatientDisease == null)
                {
                    _selectedPatient.PatientDisease = new Disease(
                        disId: 0,
                        title: SymptomTB.Text,
                        symptom: SymptomDescriptionTB.Text
                    );
                }
                else
                {
                    _selectedPatient.PatientDisease.DiseaseTitle = SymptomTB.Text;
                    _selectedPatient.PatientDisease.DiseaseSymptom = SymptomDescriptionTB.Text;
                }

                // Сохраняем изменения
                _patientService.UpdatePatient(_selectedPatient);

                // Обновляем DataGrid
                PaientsDG.ItemsSource = null;
                PaientsDG.ItemsSource = _patientService.GetAllPatients();

                MessageBox.Show("Данные пациента обновлены!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void DeletePatientBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPatient == null)
            {
                MessageBox.Show("Выберите пациента для удаления!");
                return;
            }

            var result = MessageBox.Show($"Удалить пациента {_selectedPatient.FullName}?",
                                       "Подтверждение удаления",
                                       MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _patientService.DeletePatient(_selectedPatient.PersonId);
                    LoadPatients();
                    ClearTextFields();
                    MessageBox.Show("Пациент удален!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }

        private bool ValidatePatientForm()
        {
            if (string.IsNullOrWhiteSpace(PatSurnameTB.Text) ||
                string.IsNullOrWhiteSpace(PatNameTB.Text) ||
                string.IsNullOrWhiteSpace(PatPhoneTB.Text) ||
                PatDateBirthDP.SelectedDate == null)
            {
                MessageBox.Show("Заполните обязательные поля: Фамилия, Имя, Телефон, Дата рождения");
                return false;
            }

            if (PatPhoneTB.Text.Length != 11)
            {
                MessageBox.Show("Номер телефона должен состоять из 11 цифр");
                return false;
            }

            if (string.IsNullOrWhiteSpace(SymptomTB.Text))
            {
                MessageBox.Show("Укажите симптом/жалобу пациента");
                return false;
            }

            return true;
        }

        private void ClearTextFields()
        {
            PatSurnameTB.Clear();
            PatNameTB.Clear();
            PatPatronymicTB.Clear();
            PatPhoneTB.Clear();
            PatMailTB.Clear();
            PatDateBirthDP.SelectedDate = null;
            SymptomTB.Clear();
            SymptomDescriptionTB.Clear();
            _selectedPatient = null;
            PaientsDG.SelectedItem = null;
        }

        private void ClearPatientBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearTextFields();
        }
    }
}
