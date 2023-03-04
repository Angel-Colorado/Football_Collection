using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project1Api.Data;
using Project1Api.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Project1Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerTeamsController : ControllerBase
    {
        private readonly FootballContext _context;

        public PlayerTeamsController(FootballContext context)
        {
            _context = context;
        }

        // GET: api/PlayerTeams
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerTeam>>> GetPlayerTeams()
        {
            return await _context.PlayerTeams.ToListAsync();
        }

        // GET: api/PlayerTeams/5
        [HttpGet("GetByID")]
        public async Task<ActionResult<PlayerTeam>> GetPlayerTeam(int playerID, int teamID)
        {
            var playerTeam = await _context.PlayerTeams.FindAsync(playerID, teamID);

            if (playerTeam == null)
            {
                return NotFound();
            }

            return playerTeam;
        }

        // PUT: api/PlayerTeams/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutPlayerTeam(int currentPlayerID, int currentTeamID, int newPlayerID, int newTeamID)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get the record you want to update
            var playerTeamToUpdate = await _context.PlayerTeams.FindAsync(currentPlayerID, currentTeamID);

            // Check that you got it
            if (playerTeamToUpdate == null)
            {
                return NotFound(new { message = "Error: PlayerTeam record not found" });
            }

            try
            {
                // Since the EF doesn't allow to modify FKs, the Update is executed with a raw query
                string cmd = $"UPDATE PlayerTeams SET PlayerID = {newPlayerID}, TeamID = {newTeamID} WHERE PlayerID = {currentPlayerID} AND TeamID = {currentTeamID};";
                _context.Database.ExecuteSqlRaw(cmd);

                //await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayerTeamExists(currentPlayerID, currentTeamID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception dex)   // Since it's a raw query the Exception comes as a General Exception, not a DbUpdateException
            {
                if (dex.GetBaseException().Message.Contains("PlayerLeague Trigger"))
                {
                    return BadRequest(new
                    {
                        message = "Unable to save: By assigning this Player to this Team you are attempting " +
                        "to violate the rule that a Player can only be in a League at a time"
                    });
                }
                else
                {
                    return BadRequest(new { message = "Unable to save changes to the database. Try again, and if the problem persists see your system administrator" });
                }
            }
        }

        // POST: api/PlayerTeams
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PlayerTeam>> PostPlayerTeam(int playerID, int teamID)
        {
            var playerTeamToAdd = new PlayerTeam { PlayerID = playerID, TeamID = teamID };
            
            _context.PlayerTeams.Add(playerTeamToAdd);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("PlayerLeague Trigger"))
                {
                    return BadRequest(new
                    {
                        message = "Unable to save: By assigning this Player to this Team you are attempting " +
                        "to violate the rule that a Player can only be in a League at a time"
                    });
                }
                else
                {
                    return BadRequest(new { message = "Unable to save changes to the database. Try again, and if the problem persists see your system administrator" });
                }
            }

            return CreatedAtAction("GetPlayerTeam", new { id = playerTeamToAdd.PlayerID, playerTeamToAdd.TeamID }, playerTeamToAdd);
        }

        // DELETE: api/PlayerTeams/5
        [HttpDelete]
        public async Task<IActionResult> DeletePlayerTeam(int playerID, int teamID)
        {
            var playerTeam = await _context.PlayerTeams.FindAsync(playerID, teamID);
            if (playerTeam == null)
            {
                return NotFound();
            }

            _context.PlayerTeams.Remove(playerTeam);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlayerTeamExists(int pID, int tID)
        {
            return _context.PlayerTeams.Any(e => e.PlayerID == pID && e.TeamID == tID);
        }
    }
}
