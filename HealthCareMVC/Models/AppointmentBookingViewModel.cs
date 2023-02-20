using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HealthCareMVC.Models
{
    public class AppointmentBookingViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]

        public DateTime Date { get; set; }
        [Required]

        public DateTime Time { get; set; }

       // public DocSpecialization Specialization { get; set; }
       [Display(Name ="Specialization")]
        public int DocSpecializationId { get; set; }
        [Required]

        public string SpecializationName { get; set; }
        public List<DocSpecializationModel> Specializations { get; set; }

    }
}
