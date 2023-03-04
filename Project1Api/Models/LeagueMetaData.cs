using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;
using System.Xml.Linq;
using System.Text.Json.Serialization;

namespace Project1Api.Models
{
    public class LeagueMetaData
    {
        [Key]
        [Display(Name = "Code Name")]
        [Required(ErrorMessage = "Code Name is required")]
        [RegularExpression("^[A-Z]{2}$", ErrorMessage = "Please enter a valid 2-chars uppercase code")]   // Only Upper Case
        [StringLength(2)]
        public string ID { get; set; }

        [Required(ErrorMessage = "League Name is required")]
        [StringLength(50, ErrorMessage = "League Name cannot be more than 50 characters long")]
        public string Name { get; set; }
    }
}
