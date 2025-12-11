using Online_Doctor_Appointment_System.Models.AnalyticsModels;
using Online_Doctor_Appointment_System.Repositories;
using Online_Doctor_Appointment_System.Services;
using Online_Doctor_Appointment_System.Services.AnalyticsServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Online_Doctor_Appointment_System.Views
{
    /// <summary>
    /// Логика взаимодействия для StatisticWindow.xaml
    /// </summary>
    public partial class StatisticWindow : Window
    {
        private readonly DoctorEfficiencyAnalyzer _efficiencyAnalyzer;
        public StatisticWindow()
        {
            InitializeComponent();

            XmlAppointmentRepository appointmentRepo = new XmlAppointmentRepository();
            DoctorService doctorService = new DoctorService(new XmlDoctorRepository());
            PatientService patientService = new PatientService(new XmlPatientRepository());

            _efficiencyAnalyzer = new DoctorEfficiencyAnalyzer(appointmentRepo, doctorService);
            InitializeData();
            LoadEfficiencyData();
        }

        private void InitializeData()
        {
            double rating = 0.3;
            double experience = 0.25;
            double successRate = 0.35;
            double waitTime = 0.1;
            RatingTB.Text = rating.ToString();
            ExperienceTB.Text = experience.ToString();
            CompletedRecTB.Text = successRate.ToString();
            WaitingTimeTB.Text = waitTime.ToString();

            RatingPercentText.Text = $"{(rating * 100).ToString()} %";
            ExpPercentText.Text = $"{(experience * 100).ToString()} %";
            CompRecPercentText.Text = $"{(successRate * 100).ToString()} %";
            WaitTimePercentText.Text = $"{(waitTime * 100).ToString()} %";
        }
        private void LoadEfficiencyData()
        {
            List<DoctorEfficiency> efficiencies = _efficiencyAnalyzer.CalculateEfficiency();
            EfficiencyDG.ItemsSource = efficiencies;
        }

        private void CalcStatisticBtn_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(RatingTB.Text) ||
               string.IsNullOrWhiteSpace(ExperienceTB.Text) ||
               string.IsNullOrWhiteSpace(CompletedRecTB.Text) ||
               string.IsNullOrWhiteSpace(WaitingTimeTB.Text)
                )
            {
                MessageBox.Show("Заполните все текстовые поля весовых коэффицентов", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                double rating = Convert.ToDouble(RatingTB.Text);
                double experience = Convert.ToDouble(ExperienceTB.Text);
                double successRate = Convert.ToDouble(CompletedRecTB.Text);
                double waitTime = Convert.ToDouble(WaitingTimeTB.Text);
                double coefficentsSum = rating + experience + successRate + waitTime;
                if (coefficentsSum != 1)
                {
                    MessageBox.Show("Сумма всех весовых коэффицентов должна быть равна 1", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                RatingPercentText.Text = $"{(rating * 100).ToString()} %";
                ExpPercentText.Text = $"{(experience * 100).ToString()} %";
                CompRecPercentText.Text = $"{(successRate * 100).ToString()} %";
                WaitTimePercentText.Text = $"{(waitTime * 100).ToString()} %";

                List<DoctorEfficiency> efficiencies = _efficiencyAnalyzer.CalculateEfficiency(rating, experience, successRate, waitTime);
                EfficiencyDG.ItemsSource = efficiencies;
            }
            catch(FormatException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void HelpBtn_Click(object sender, RoutedEventArgs e)
        {
            Popup popup = HelpBtn.FindResource("HelpPopup") as Popup;
            if (popup != null)
            {
                // Рассчитываем центр экрана
                var mainWindow = Application.Current.MainWindow;
                popup.Placement = PlacementMode.Center;
                popup.PlacementTarget = mainWindow;
                popup.IsOpen = !popup.IsOpen;
            }
        }

        private void ClosePopup_Click(object sender, RoutedEventArgs e)
        {
            Popup popup = HelpBtn.FindResource("HelpPopup") as Popup;
            if (popup != null)
            {
                popup.IsOpen = false;
            }
        }
    }
}
