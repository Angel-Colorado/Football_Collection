using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;
using System.Xml.Linq;
using System.Text.Json.Serialization;

namespace Project1Api.Models
{
    [ModelMetadataType(typeof(LeagueMetaData))]
    public class League : Auditable
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public ICollection<Team> Teams { get; set; } = new HashSet<Team>();
    }
}
