using Online_Doctor_Appointment_System.Models;
using Online_Doctor_Appointment_System.Views;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Online_Doctor_Appointment_System
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void MedicalWorkersBtn_Click(object sender, RoutedEventArgs e)
        {
            DoctorWindow docWin = new DoctorWindow();
            docWin.Show();
        }

        private void PatientBtn_Click(object sender, RoutedEventArgs e)
        {
            PatientWindow patWindow = new PatientWindow();
            patWindow.Show();
        }

        private void MedicalAppointmentsBtn_Click(object sender, RoutedEventArgs e)
        {
            AppointmentWindow appWin = new AppointmentWindow();
            appWin.Show();
        }

        private void Statistic_Click(object sender, RoutedEventArgs e)
        {
            StatisticWindow statWin = new StatisticWindow();
            statWin.Show();
        }
    }
}
