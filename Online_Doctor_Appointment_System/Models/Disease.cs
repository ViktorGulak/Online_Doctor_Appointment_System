using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online_Doctor_Appointment_System.Models
{
    class Disease
    {
        private long diseaseId;
        private string title;
        private string symptom; 

        public Disease(long disId, string title, string symptom)
        {
            DiseaseId = disId;
            Title = title;
            Symptom = symptom;
        }

        public long DiseaseId
        {
            get => diseaseId;
            set
            {
                if (value < 0) throw new ArgumentException("Идентификатор болезни не может быть меньше 0.");
                diseaseId = value;
            }
        }

        public string Title
        {
            get => title;
            set => title = value;
        }

        public string Symptom
        {
            get => symptom;
            set => symptom = value;
        }
    }
}
