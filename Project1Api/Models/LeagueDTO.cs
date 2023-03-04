using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;
using System.Xml.Linq;
using System.Text.Json.Serialization;

namespace Project1Api.Models
{
    [ModelMetadataType(typeof(LeagueMetaData))]
    public class LeagueDTO
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public int? NumberOfTeams { get; set; } = null;

        public ICollection<TeamDTO> Teams { get; set; }
    }
}
