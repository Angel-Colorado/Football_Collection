using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Security.AccessControl;
using System.Xml.Linq;
using System.Text.Json.Serialization;

namespace Project1Api.Models
{
    [ModelMetadataType(typeof(TeamMetaData))]
    public class TeamDTO : IValidatableObject
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public double Budget { get; set; }

        public int? NumberOfPlayers { get; set; } = null;

        public string LeagueID { get; set; }

        public LeagueDTO League { get; set; }

        public ICollection<PlayerTeam> PlayerTeams { get; set; }

        public ICollection<PlayerDTO> Players { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            if (Name[0] == 'X' || Name[0] == 'F' || Name[0] == 'S')
            {
                yield return new ValidationResult("Team names are not allowed to start with the letters X, F, or S", new[] { "Name" });
            }
        }
    }
}
