using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System.Text.Json.Serialization;

namespace Project1Api.Models
{
    [ModelMetadataType(typeof(PlayerMetaData))]
    public class PlayerDTO
    {
        public int ID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Jersey { get; set; }

        public DateTime DOB { get; set; }

        public double FeePaid { get; set; }

        public string EMail { get; set; }

        public Byte[] RowVersion { get; set; }

        public ICollection<PlayerTeam> PlayerTeams { get; set; }

        public ICollection<TeamDTO> Teams { get; set; }
    }
}
