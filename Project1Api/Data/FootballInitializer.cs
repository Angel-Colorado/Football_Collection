using Microsoft.EntityFrameworkCore;
using Project1Api.Models;
using System.Diagnostics;
using System.Numerics;
using System.Xml.Linq;

namespace Project1Api.Data
{
    public static class FootballInitializer
    {
        private const int NUM_PLAYERS = 50; // Number of Players to be created
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            FootballContext context = applicationBuilder.ApplicationServices.CreateScope()
                .ServiceProvider.GetRequiredService<FootballContext>();
            try
            {
                //Delete the database if you need to apply a new Migration
                //context.Database.EnsureDeleted();
                //Create the database if it does not exist and apply the Migration
                context.Database.Migrate();

                // To randomly generate data
                Random random = new Random();

                // Look for any Leagues. Since we can't have Teams without Leagues.
                if (!context.Leagues.Any())
                {
                    context.Leagues.AddRange(
                     new League
                     {
                         ID = "WC",
                         Name = "World Cup"
                     },
                     new League
                     {
                         ID = "BU",
                         Name = "Bundesliga"
                     },
                     new League
                     {
                         ID = "SA",
                         Name = "Serie A"
                     },
                     new League
                     {
                         ID = "EN",
                         Name = "English Football League"
                     });
                    context.SaveChanges();
                }

                if (!context.Teams.Any())
                {
                    // Gets the list of league IDs
                    string[] leagueIDs = context.Leagues.Select(l => l.ID).ToArray();
                    int leagueIDcount = leagueIDs.Length;

                    string[] teamNames = new string[] { "Badgers", "Bengals", "Royals", "Chili Peppers", "Cereal Killers", "Abusement Park", "Aztecs", "Red Dragons", "The Surge", "Demon Deacons", "Big Blues", "Bisons", "Golden Knights", "Bandits", "Bantams", "Phantoms", "Red Foxes", "Brigade", "Blue Typhoons", "Celtic Ladies", "Cheetahs", "Majestics", "Lady Cougars", "Black Antelopes", "Black Stars", "Dazzle", "Amigos", "Green Wave", "Boilermakers", "Bombers", "Colonels", "Chippewas", "Catamounts", "Crimson Tide", "Orange Crush", "Crushers", "Dancing Divas", "Demolition Day", "Musketeers" };
                    int teamNamesCount = teamNames.Length;

                    // Generates a set of random strings, this way it will avoid to repeat Team Names
                    HashSet<string> randomNames = new HashSet<string>();

                    if (teamNames.Length >= 10)          // Checks one time if the array of names is equal or larger than 10
                    {
                        while (randomNames.Count != 10)  // Until the count reaches 10
                        {
                            randomNames.Add(teamNames[random.Next(teamNamesCount)]);    // Picks a random Team Name
                        }
                    }

                    // Loops through the list of Team Names and builds the Team item as we go
                    foreach (string t in randomNames)
                    {
                        Team tempTeam = new Team()
                        {
                            Name = t,
                            Budget = random.Next(50, 1000) * 10,
                            LeagueID = leagueIDs[random.Next(leagueIDcount)]
                        };
                        context.Teams.Add(tempTeam);
                    }
                    context.SaveChanges();
                }

                // Enter 10 x 12 = 120
                if (!context.Players.Any())
                {
                    string[] firstNames = new string[] { "Sergei", "Janine", "Anna", "Erik", "Heloise", "Alyssa", "Enrico", "Claude", "Franz", "Terrence" };
                    int firstNamesCount = firstNames.Length;

                    string[] lastNames = new string[] { "Rachmaninov", "Debussy", "Satie", "Prokofiev", "Bach", "Tchaikovsky", "Fedorova", "Chopin", "Vivaldi", "Horowitz", "Liszt", "Strauss" };
                    int lastNamesCount = lastNames.Length;

                    string[] emailDomains = new string[] { "hotmail", "outlook", "gmail", "yahoo", "aol" };

                    // Generates a set of random strings, this way it will avoid to repeat Jersey numbers
                    HashSet<string> randomJerseys = new HashSet<string>();

                    while (randomJerseys.Count != NUM_PLAYERS)   // Until the count reaches the number
                    {
                        randomJerseys.Add(random.Next(10, 100).ToString()); // Generates a random Jersey number
                    }
                    var listJerseys = randomJerseys.ToList();
                    int i = 0;

                    // Generates a set of random strings, this way it will avoid to repeat Player Names, which is not required
                    HashSet<string> randomPlayers = new HashSet<string>();

                    string[] player = new string[2];    // Creates an array with size of 2

                    while (randomPlayers.Count != NUM_PLAYERS)   // Until the count reaches number
                    {
                        string fName = firstNames[random.Next(firstNamesCount)];
                        string lName = lastNames[random.Next(lastNamesCount)];

                        // If the player is successfully added to the HashSet it means is not duplicated
                        if (randomPlayers.Add(fName + lName))   
                        {   // Then creates the player
                            Player tempPlayer = new Player()
                            {
                                FirstName = fName,          // Gets the First Name
                                LastName = lName,           // Gets the Last Name
                                Jersey = listJerseys[i++],  // Gets the Jersey Number, then increments the counter by 1
                                DOB = DateTime.Today.AddDays(-random.Next(6 * 365, 40 * 365)),  // Between ~6 and ~40 years
                                FeePaid = random.Next(140, 300),
                                EMail = (fName + "." + lName + "@" + emailDomains[random.Next(emailDomains.Length)] + ".com").ToLower()  // Builds the e-mail from the name, there it'll be unique
                            };
                            context.Players.Add(tempPlayer);
                        }
                    }
                    context.SaveChanges();
                }

                // PlayerTeams
                if (!context.PlayerTeams.Any())
                {
                    // Gets the Player and Teams IDs
                    int[] playerIDs = context.Players.Select(p => p.ID).ToArray();
                    int[] teamIDs = context.Teams.Select(t => t.ID).ToArray();

                    // All the players are assigned to a random Team
                    foreach (int p in playerIDs)
                    {
                        PlayerTeam pt = new PlayerTeam()
                        {
                            PlayerID = p,
                            TeamID = teamIDs[random.Next(teamIDs.Length)]
                        };
                        context.PlayerTeams.Add(pt);
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.GetBaseException().Message);
            }
        }
    }
}
