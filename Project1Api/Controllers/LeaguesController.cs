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
    public class LeaguesController : ControllerBase
    {
        private readonly FootballContext _context;

        public LeaguesController(FootballContext context)
        {
            _context = context;
        }

        // GET: api/Leagues
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeagueDTO>>> GetLeagues()
        {
            var leagueDTOs = await _context.Leagues
                .Select(l => new LeagueDTO
                {
                    ID = l.ID,
                    Name = l.Name
                })
                .ToListAsync();

            if (leagueDTOs.Count() > 0)
            {
                return leagueDTOs;
            }
            else
            {
                return NotFound(new { message = "Error: No League records" });
            }
        }

        // GET: api/Leagues
        [HttpGet("NumTeams")]
        public async Task<ActionResult<IEnumerable<LeagueDTO>>> GetLeaguesNumTeams()
        {
            var leagueDTOs = await _context.Leagues
                .Include(e => e.Teams)
                .Select(l => new LeagueDTO
                {
                    ID = l.ID,
                    Name = l.Name,
                    NumberOfTeams = l.Teams.Count
                })
                .ToListAsync();

            if (leagueDTOs.Count() > 0)
            {
                return leagueDTOs;
            }
            else
            {
                return NotFound(new { message = "Error: No League records" });
            }
        }

        // GET: api/Leagues
        [HttpGet("ListTeams")]
        public async Task<ActionResult<IEnumerable<LeagueDTO>>> GetLeaguesListTeams()
        {
            var leagueDTOs = await _context.Leagues
                .Include(e => e.Teams)
                .Select(l => new LeagueDTO
                {
                    ID = l.ID,
                    Name = l.Name,
                    Teams = l.Teams.Select(t => new TeamDTO
                    {
                        ID = t.ID,
                        Name = t.Name,
                        Budget = t.Budget
                    }).ToList()
                })
                .ToListAsync();

            if (leagueDTOs.Count() > 0)
            {
                return leagueDTOs;
            }
            else
            {
                return NotFound(new { message = "Error: No League records" });
            }
        }

        // GET: api/Leagues/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LeagueDTO>> GetLeague(string id)
        {
            var league = await _context.Leagues
                .Select(l => new LeagueDTO
                {
                    ID = l.ID,
                    Name = l.Name,
                })
                .FirstOrDefaultAsync(l => l.ID == id);

            if (league == null)
            {
                return NotFound(new { message = "Error: League record not found" });
            }

            return league;
        }

        // PUT: api/Leagues/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLeague(string id, LeagueDTO leagueDTO)
        {
            if (id != leagueDTO.ID)
            {
                return BadRequest(new { message = "Error: ID does not match League" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get the record you want to update
            var leagueToUpdate = await _context.Leagues.FindAsync(id);

            // Check that you got it
            if (leagueToUpdate == null)
            {
                return NotFound(new { message = "Error: League record not found" });
            }

            // Update the properties of the entity object from the DTO object
            leagueToUpdate.ID = leagueDTO.ID;
            leagueToUpdate.Name = leagueDTO.Name;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeagueExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { message = "Unable to save changes to the database. Try again, and if the problem persists see your system administrator" });
            }
        }

        // POST: api/Leagues
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LeagueDTO>> PostLeague(LeagueDTO leagueDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //League league = new League { };   //Right click in { } and Fix with MappingGenerator

            League league = new League
            {
                ID = leagueDTO.ID,
                Name = leagueDTO.Name
            };


            try
            {
                _context.Leagues.Add(league);
                await _context.SaveChangesAsync();

                // Assign Database Generated values back into the DTO. ID and Row Version?
                leagueDTO.ID = league.ID;

                return CreatedAtAction(nameof(GetLeague), new { id = league.ID }, leagueDTO);
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { message = "Unable to save changes to the database. Try again, and if the problem persists see your system administrator" });
            }
        }

        // DELETE: api/Leagues/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<League>> DeleteLeague(string id)
        {
            var league = await _context.Leagues.FindAsync(id);
            if (league == null)
            {
                return NotFound(new { message = "Delete Error: League has already been removed" });
            }

            try
            {
                _context.Leagues.Remove(league);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { message = "Delete Error: Unable to delete League" });
            }
        }

        private bool LeagueExists(string id)
        {
            return _context.Leagues.Any(e => e.ID == id);
        }
    }
}
