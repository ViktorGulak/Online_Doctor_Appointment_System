using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Online_Doctor_Appointment_System.Models
{
    [Serializable]
    [XmlRoot("Specialization")]
    public class Specialization
    {
        private long specializationId;
        private string title;
        private string description;

        public Specialization()
        {
        }

        public Specialization(long specId, string title, string description)
        {
            SpecializationId = specId;
            SpecializationTitle = title;
            SpecializationDescription = description;
        }

        public long SpecializationId
        {
            get => specializationId;
            set => specializationId = value;
        }

       
        public string SpecializationTitle
        {
            get => title;
            set => title = value;
        }

        
        public string SpecializationDescription
        {
            get => description;
            set => description = value;
        }

        public override string ToString()
        {
            return SpecializationTitle;
        }
    }
}
