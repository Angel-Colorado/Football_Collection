using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project1Api.Data;
using Project1Api.Models;

namespace Project1Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly FootballContext _context;

        public TeamsController(FootballContext context)
        {
            _context = context;
        }

        // GET: api/Teams
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamDTO>>> GetTeams()
        {
            var teamDTOs = await _context.Teams
                .Include(l => l.League)
                .Select(t => new TeamDTO
                {
                    ID = t.ID,
                    Name = t.Name,
                    Budget = t.Budget,
                    League = new LeagueDTO
                    {
                        ID = t.League.ID,
                        Name = t.League.Name
                    }
                })
                .ToListAsync();

            if (teamDTOs.Count() > 0)
            {
                return teamDTOs;
            }
            else
            {
                return NotFound(new { message = "Error: No Team records" });
            }
        }

        // GET: api/Teams
        [HttpGet("NumPlayers")]
        public async Task<ActionResult<IEnumerable<TeamDTO>>> GetTeamsNumPlayers()
        {
            var teamDTOs = await _context.Teams
                .Include(l => l.League)
                .Include(p => p.PlayerTeams)
                .Select(t => new TeamDTO
                {
                    ID = t.ID,
                    Name = t.Name,
                    Budget = t.Budget,
                    NumberOfPlayers = t.PlayerTeams.Count,
                    LeagueID = t.LeagueID,
                    League = new LeagueDTO
                    {
                        ID = t.League.ID,
                        Name = t.League.Name
                    }
                })
                .ToListAsync();

            if (teamDTOs.Count() > 0)
            {
                return teamDTOs;
            }
            else
            {
                return NotFound(new { message = "Error: No Team records" });
            }
        }

        // GET: api/Teams
        [HttpGet("ListPlayers")]
        public async Task<ActionResult<IEnumerable<TeamDTO>>> GetTeamsListPlayers()
        {
            var teamDTOs = await _context.Teams
                .Include(l => l.League)
                .Include(p => p.PlayerTeams).ThenInclude(p => p.Player)
                .Select(t => new TeamDTO
                {
                    ID = t.ID,
                    Name = t.Name,
                    Budget = t.Budget,
                    LeagueID = t.LeagueID,
                    League = new LeagueDTO
                    {
                        ID = t.League.ID,
                        Name = t.League.Name,
                    },
                    Players = t.PlayerTeams.Select(pl => new PlayerDTO
                    {
                        ID = pl.Player.ID,
                        FirstName = pl.Player.FirstName,
                        LastName = pl.Player.LastName,
                        Jersey = pl.Player.Jersey,
                        DOB = pl.Player.DOB,
                        FeePaid = pl.Player.FeePaid,
                        EMail = pl.Player.EMail
                    }).ToList()
                })
                .ToListAsync();

            if (teamDTOs.Count() > 0)
            {
                return teamDTOs;
            }
            else
            {
                return NotFound(new { message = "Error: No Team records" });
            }
        }

        // GET: api/Teams
        [HttpGet("NumPlayersByLeague/{id}")]
        public async Task<ActionResult<IEnumerable<TeamDTO>>> GetTeamsNumPlayersByLeague(string id)
        {
            var teamDTOs = await _context.Teams
                .Include(l => l.League)
                .Include(p => p.PlayerTeams)
                .Select(t => new TeamDTO
                {
                    ID = t.ID,
                    Name = t.Name,
                    Budget = t.Budget,
                    NumberOfPlayers = t.PlayerTeams.Count,
                    LeagueID = t.LeagueID,
                    League = new LeagueDTO
                    {
                        ID = t.League.ID,
                        Name = t.League.Name
                    }
                })
                .Where(t => t.LeagueID == id)
                .ToListAsync();

            if (teamDTOs.Count() > 0)
            {
                return teamDTOs;
            }
            else
            {
                return NotFound(new { message = "Error: No Team records for that League" });
            }
        }

        // GET: api/Teams
        [HttpGet("ListPlayersByLeague/{id}")]
        public async Task<ActionResult<IEnumerable<TeamDTO>>> GetTeamsListPlayersByLeague(string id)
        {
            var teamDTOs = await _context.Teams
                .Include(l => l.League)
                .Include(p => p.PlayerTeams)
                .Select(t => new TeamDTO
                {
                    ID = t.ID,
                    Name = t.Name,
                    Budget = t.Budget,
                    NumberOfPlayers = t.PlayerTeams.Count,
                    LeagueID = t.LeagueID,
                    League = new LeagueDTO
                    {
                        ID = t.League.ID,
                        Name = t.League.Name
                    },
                    Players = t.PlayerTeams.Select(pl => new PlayerDTO
                    {
                        ID = pl.Player.ID,
                        FirstName = pl.Player.FirstName,
                        LastName = pl.Player.LastName,
                        Jersey = pl.Player.Jersey,
                        DOB = pl.Player.DOB,
                        FeePaid = pl.Player.FeePaid,
                        EMail = pl.Player.EMail
                    }).ToList(),
                })
                .Where(t => t.LeagueID == id)
                .ToListAsync();

            if (teamDTOs.Count() > 0)
            {
                return teamDTOs;
            }
            else
            {
                return NotFound(new { message = "Error: No Team records for that League" });
            }
        }

        // GET: api/Teams/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TeamDTO>> GetTeam(int id)
        {
            var team = await _context.Teams
                .Select(l => new TeamDTO
                {
                    ID = l.ID,
                    Name = l.Name,
                    Budget = l.Budget,
                    League = new LeagueDTO
                    {
                        ID = l.League.ID,
                        Name = l.League.Name
                    }
                })
                .FirstOrDefaultAsync(l => l.ID == id);

            if (team == null)
            {
                return NotFound(new { message = "Error: Team record not found" });
            }

            return team;
        }

        // PUT: api/Teams/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeam(int id, TeamDTO teamDTO)
        {
            if (id != teamDTO.ID)
            {
                return BadRequest(new { message = "Error: ID does not match Team" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get the record you want to update
            var teamToUpdate = await _context.Teams.FindAsync(id);

            // Check that you got it
            if (teamToUpdate == null)
            {
                return NotFound(new { message = "Error: Team record not found" });
            }

            //teamToUpdate = teamDTO; //- Fix with MappingGenerator
            //Generate explicit conversion (Try re-use instance)

            // Update the properties of the entity object from the DTO object
            teamToUpdate.ID = teamDTO.ID;
            teamToUpdate.Name = teamDTO.Name;
            teamToUpdate.Budget = teamDTO.Budget;
            teamToUpdate.LeagueID = teamDTO.LeagueID;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeamExists(id))
                {
                    return Conflict(new { message = "Concurrency Error: Team has been Removed." });
                }
                else
                {
                    return Conflict(new { message = "Concurrency Error: Team has been updated by another user.  Back out and try editing the record again." });
                }
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("PlayerLeague Trigger"))
                {
                    return BadRequest(new
                    {
                        message = "Unable to save: By moving this Team you are attempting to " +
                        "violate the rule that a Player can only be in a League at a time"
                    });
                }
                else
                {
                    return BadRequest(new { message = "Unable to save changes to the database. Try again, and if the problem persists see your system administrator" });
                }
            }
        }

        // POST: api/Teams
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TeamDTO>> PostTeam(TeamDTO teamDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Team team = new Team { };   //Right click in { } and Fix with MappingGenerator

            Team team = new Team
            {
                ID = teamDTO.ID,
                Name = teamDTO.Name,
                Budget = teamDTO.Budget,
                LeagueID = teamDTO.LeagueID,
            };

            try
            {
                _context.Teams.Add(team);
                await _context.SaveChangesAsync();

                // Assign Database Generated values back into the DTO. ID and Row Version?
                teamDTO.ID = team.ID;


                return CreatedAtAction(nameof(GetTeam), new { id = team.ID }, teamDTO);
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { message = "Unable to save changes to the database. Try again, and if the problem persists see your system administrator" });
            }
        }

        // DELETE: api/Teams/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Team>> DeleteTeam(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound(new { message = "Delete Error: Team has already been removed" });
            }

            try
            {
                _context.Teams.Remove(team);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { message = "Delete Error: Unable to delete Team. The Team probably has Players associated to it" });
            }
        }

        private bool TeamExists(int id)
        {
            return _context.Teams.Any(e => e.ID == id);
        }
    }
}
