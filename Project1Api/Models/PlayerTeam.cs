using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace Project1Api.Models
{
    public class PlayerTeam
    {
        public int PlayerID { get; set; }
        public Player Player { get; set; }

        public int TeamID { get; set; }
        public Team Team { get; set; }

    }
}
