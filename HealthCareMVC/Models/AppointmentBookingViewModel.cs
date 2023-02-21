using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HealthCareMVC.Models
{
    public class AppointmentBookingViewModel
    {

        
        public int Id { get; set; }

        public string PatientName { get; set; }
        public int Age { get; set; }

        public string HealthIssue { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime Date { get; set; }

    

        public DateTime Time { get; set; }

       // public DocSpecialization Specialization { get; set; }
       [Display(Name ="Specialization")]
        public int DocSpecializationId { get; set; }

     

        public string SpecializationName { get; set; }
        public List<DocSpecializationModel> Specializations { get; set; }

    }
}
