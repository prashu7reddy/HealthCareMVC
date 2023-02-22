using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HealthCareMVC.Models
{
    public class DoctorViewModel
    {
        public int Id { get; set; }
       // [Required]

        public string DoctorName { get; set; }
        public int DocSpecializationId { get; set; }
        // public DocSpecializationModel Specialization { get; set; }
       // [Required]

        public string Specialization { get; set; }
       // [Required]

        public string PhoneNumber { get; set; }
       // [Required]

        public string EmailId { get; set; }
        public List<DocSpecializationModel> Specializations { get; set; }
    }
}
