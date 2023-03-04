using Microsoft.EntityFrameworkCore.Migrations;

namespace Project1Api.Data
{
    public static class ExtraMigration
    {
        public static void Steps(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetPlayerTimestampOnUpdate
                    AFTER UPDATE ON Players
                    BEGIN
                        UPDATE Players
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetPlayerTimestampOnInsert
                    AFTER INSERT ON Players
                    BEGIN
                        UPDATE Players
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");

// The following triggers are set up to detect when a Player is in a League more than once, which
//  is not allowed, therefore the Update or Create will be rolled back. These methods are
//  triggered in the following cases:
//      - Team Update. By changing the LeagueID
//      - PlayerTeam Insert. By assigning a PlayerID to a TeamID in the same league
//      - PlayerTeam Update.  ---same---

// Since The PlayerTeams Controller is not a requirement, it is scaffolded to maintain the basic
//  tasks. You can see there with Swagger how the Exceptions show up when the user Insert or Update
//  a Player in more than one in a League at a time

            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER CheckPlayerLeagueOnTeamUpdate
                    AFTER UPDATE ON Teams
                    BEGIN
                    SELECT CASE
                        WHEN EXISTS (SELECT LP.LeagueID, LP.PlayerID
                            FROM (Select T.LeagueID as LeagueID, PT.PlayerID as PlayerID, T.Name as TeamName
                                From Teams T Join PlayerTeams PT   
                                    on T.ID = PT.TeamID) LP
                                GROUP BY LeagueID, PlayerID
                                HAVING COUNT(*) > 1)
                    THEN RAISE(ABORT, 'PlayerLeague Trigger. You are attempting to violate the rule that a Player can only be in a League at a time')
                    END;
                    END;
                ");
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER CheckPlayerLeagueOnPlayerTeamInsert
                    AFTER INSERT ON PlayerTeams
                    BEGIN
                    SELECT CASE
                        WHEN EXISTS (SELECT LP.LeagueID, LP.PlayerID
                            FROM (Select T.LeagueID as LeagueID, PT.PlayerID as PlayerID, T.Name as TeamName
                                From Teams T Join PlayerTeams PT   
                                    on T.ID = PT.TeamID) LP
                                GROUP BY LeagueID, PlayerID
                                HAVING COUNT(*) > 1)
                    THEN RAISE(ABORT, 'PlayerLeague Trigger. You are attempting to violate the rule that a Player can only be in a League at a time')
                    END;
                    END;
                ");
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER CheckPlayerLeagueOnPlayerTeamUpdate
                    AFTER UPDATE ON PlayerTeams
                    BEGIN
                    SELECT CASE
                        WHEN EXISTS (SELECT LP.LeagueID, LP.PlayerID
                            FROM (Select T.LeagueID as LeagueID, PT.PlayerID as PlayerID, T.Name as TeamName
                                From Teams T Join PlayerTeams PT   
                                    on T.ID = PT.TeamID) LP
                                GROUP BY LeagueID, PlayerID
                                HAVING COUNT(*) > 1)
                    THEN RAISE(ABORT, 'PlayerLeague Trigger. You are attempting to violate the rule that a Player can only be in a League at a time')
                    END;
                    END;
                ");
        }
    }
}
