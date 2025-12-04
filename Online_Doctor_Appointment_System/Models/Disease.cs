using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Online_Doctor_Appointment_System.Models
{
    [Serializable]
    [XmlRoot("Disease")]
    public class Disease
    {
        private long diseaseId;
        private string title;
        private string symptom;

        // Конструктор для сериализации
        public Disease() { }
        public Disease(long disId, string title, string symptom)
        {
            DiseaseId = disId;
            DiseaseTitle = title;
            DiseaseSymptom = symptom;
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

        [XmlElement("Title")]
        public string DiseaseTitle
        {
            get => title;
            set => title = value;
        }

        [XmlElement("Symptom")]
        public string DiseaseSymptom
        {
            get => symptom;
            set => symptom = value;
        }
    }
}
