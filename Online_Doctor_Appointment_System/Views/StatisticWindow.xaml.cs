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
        //private readonly LoadForecastAnalyzer _forecastAnalyzer;
        //private readonly PatientClusteringAnalyzer _clusteringAnalyzer;
        public StatisticWindow()
        {
            InitializeComponent();

            XmlAppointmentRepository appointmentRepo = new XmlAppointmentRepository();
            DoctorService doctorService = new DoctorService(new XmlDoctorRepository());
            PatientService patientService = new PatientService(new XmlPatientRepository());

            _efficiencyAnalyzer = new DoctorEfficiencyAnalyzer(appointmentRepo, doctorService);
            // _forecastAnalyzer = new LoadForecastAnalyzer(appointmentRepo);
            //_clusteringAnalyzer = new PatientClusteringAnalyzer(patientService);
            LoadEfficiencyData();
        }

        private void LoadEfficiencyData()
        {
            var efficiencies = _efficiencyAnalyzer.CalculateEfficiency();
            EfficiencyDG.ItemsSource = efficiencies;
        }
    }
}
