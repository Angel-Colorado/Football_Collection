using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Security.AccessControl;
using System.Xml.Linq;
using System.Text.Json.Serialization;

namespace Project1Api.Models
{
    public class TeamMetaData
    {
        [Display(Name = "Team Name")]
        [Required(ErrorMessage = "You cannot leave the team name blank")]
        [StringLength(70, ErrorMessage = "Team name cannot be more than 70 characters long")]
        public string Name { get; set; }

        [Required(ErrorMessage = "You cannot leave the Budget blank")]
        [Range(500.0, 10000.0, ErrorMessage = "Budget must be between $500 and $10,000")]
        [DataType(DataType.Currency)]
        public double Budget { get; set; }

        [Required(ErrorMessage = "You must select a League")]
        [Display(Name = "League")]
        public string LeagueID { get; set; }

        [Display(Name = "Other Players")]
        public ICollection<PlayerTeam> PlayerTeams { get; set; } = new HashSet<PlayerTeam>();
    }
}
