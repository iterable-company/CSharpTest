using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Football;

namespace CSharpTest
{
    [TestClass]
    public class EntityFrameworkTest
    {
        public static void CleanUp(FootballDbContext dbContext)
        {
            dbContext.Database.ExecuteSqlCommand("DELETE FROM Players");
            dbContext.Database.ExecuteSqlCommand("DELETE FROM Countries");
            dbContext.Database.ExecuteSqlCommand("DELETE FROM Clubs");
            dbContext.Database.ExecuteSqlCommand("DELETE FROM Cities");
            dbContext.Database.ExecuteSqlCommand("DELETE FROM Leagues");

            dbContext.Database.ExecuteSqlCommand("DBCC CHECKIDENT ('Players', RESEED, 0)");
            dbContext.Database.ExecuteSqlCommand("DBCC CHECKIDENT ('Countries', RESEED, 0)");
            dbContext.Database.ExecuteSqlCommand("DBCC CHECKIDENT ('Clubs', RESEED, 0)");
            dbContext.Database.ExecuteSqlCommand("DBCC CHECKIDENT ('Cities', RESEED, 0)");
            dbContext.Database.ExecuteSqlCommand("DBCC CHECKIDENT ('Leagues', RESEED, 0)");

        }
        public static void SetupFixture(FootballDbContext dbContext)
        {
            League premier = new League()
            {
                Name = "Premier"
            };
            League league1 = new League()
            {
                Name = "League1"
            };
            League bundesleaga = new League()
            {
                Name = "Bundesleaga"
            };
            League laleaga = new League()
            {
                Name = "LaLeaga"
            };

            City southamptonCity = new City()
            {
                Name = "Southampton"
            };
            City leicesterCity = new City()
            {
                Name = "Leicester"
            };
            City marseilleCity = new City()
            {
                Name = "Marseille"
            };
            City eibarCity = new City()
            {
                Name = "Eibar"
            };

            Club southampton = new Club()
            {
                Name = "Southampton FC",
                League = premier,
                City = southamptonCity
            };
            Club leicester = new Club()
            {
                Name = "Leicester City FC",
                League = premier,
                City = leicesterCity
            };
            Club marseille = new Club()
            {
                Name = "Olympic Marseille",
                League = league1,
                City = marseilleCity
            };
            Club eibar = new Club()
            {
                Name = "SD Eibar",
                League = laleaga,
                City = eibarCity
            };

            Player yoshida_maya = new Player()
            {
                Name = "YOSHIDA Maya",
                Position = "DF",
                Club = southampton
            };
            Player okazaki_shinji = new Player()
            {
                Name = "OKAZAKI Shinji",
                Position = "FW",
                Club = leicester
            };
            Player sakai_hiroki = new Player()
            {
                Name = "SAKAI Hiroki",
                Position = "DF",
                Club = marseille
            };
            Player inui_takashi = new Player()
            {
                Name = "INUI Takashi",
                Position = "FW",
                Club = eibar
            };

            dbContext.Players.Add(yoshida_maya);
            dbContext.Players.Add(okazaki_shinji);
            dbContext.Players.Add(sakai_hiroki);
            dbContext.Players.Add(inui_takashi);

            dbContext.SaveChanges();
        }
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            using (var dbContext = new FootballDbContext())
            {
                CleanUp(dbContext);
                SetupFixture(dbContext);
            }
        }
        [TestMethod]
        public void TestWhereUsingInclude()
        {
            using (var dbContext = new FootballDbContext())
            {
                dbContext.Database.Log = Console.WriteLine;
                List<Player> toBeTested = dbContext.Players.Where(x => x.Name.Contains("YOSHIDA")).Include(x => x.Club).Include(x => x.Club.City).ToList();
                Assert.AreEqual(toBeTested.Count, 1);

                Player yoshida = toBeTested[0];
                Assert.AreEqual(yoshida.Name, "YOSHIDA Maya");
                Assert.AreEqual(yoshida.Position, "DF");
                Assert.AreEqual(yoshida.Club.Name, "Southampton FC");
                Assert.AreEqual(yoshida.Club.City.Name, "Southampton");
            }
        }
        [TestMethod]
        public void TestWhereUsingIncludeCityOnly()
        {
            using (var dbContext = new FootballDbContext())
            {
                dbContext.Database.Log = Console.WriteLine;
                List<Player> toBeTested = dbContext.Players.Where(x => x.Name.Contains("YOSHIDA")).Include(x => x.Club).ToList();
                Assert.AreEqual(toBeTested.Count, 1);

                Player yoshida = toBeTested[0];
                Assert.AreEqual(yoshida.Name, "YOSHIDA Maya");
                Assert.AreEqual(yoshida.Position, "DF");
                Assert.AreEqual(yoshida.Club.Name, "Southampton FC");
                Assert.AreEqual(yoshida.Club.City.Name, "Southampton");
            }
        }
        [TestMethod]
        public void TestWhereNotUsingInclude()
        {
            using (var dbContext = new FootballDbContext())
            {
                dbContext.Database.Log = Console.WriteLine;
                List<Player> toBeTested = dbContext.Players.Where(x => x.Name.Contains("YOSHIDA")).ToList();
                Assert.AreEqual(toBeTested.Count, 1);

                Player yoshida = toBeTested[0];
                Assert.AreEqual(yoshida.Name, "YOSHIDA Maya");
                Assert.AreEqual(yoshida.Position, "DF");
                Assert.AreEqual(yoshida.Club.Name, "Southampton FC");
                Assert.AreEqual(yoshida.Club.City.Name, "Southampton");
            }
        }
        [TestMethod]
        public void TestFind()
        {
            using (var dbContext = new FootballDbContext())
            {
                dbContext.Database.Log = Console.WriteLine;
                Player toBeTested = dbContext.Players
                    .Find(x => x.Name.Contains("S"))
                    .Some(x => x)
                    .None(new Player() { });
                Assert.AreEqual(toBeTested.Name, "YOSHIDA Maya");
                Assert.AreEqual(toBeTested.Position, "DF");
                Assert.AreEqual(toBeTested.Club.Name, "Southampton FC");
                Assert.AreEqual(toBeTested.Club.City.Name, "Southampton");
            }
        }
        [TestMethod]
        public void TestFindWhereManyHit()
        {
            using (var dbContext = new FootballDbContext())
            {
                dbContext.Database.Log = Console.WriteLine;

                Player toBeTested = dbContext.Players
                    .Where(x => x.Name.Contains("S"))
                    .Include(x => x.Club)
                    .Include(x => x.Club.City)
                    .OrderBy(x => -1 * x.Id)
                    .Find(x => x.Name.Contains("S"))
                    .Some(x => x)
                    .None(new Player() { });
                Assert.AreEqual(toBeTested.Name, "SAKAI Hiroki");
                Assert.AreEqual(toBeTested.Position, "DF");
                Assert.AreEqual(toBeTested.Club.Name, "Olympic Marseille");
                Assert.AreEqual(toBeTested.Club.City.Name, "Marseille");
            }
        }
        [ClassCleanup]
        public static void ClassTeardown()
        {
            using (var dbContext = new FootballDbContext())
            {
                CleanUp(dbContext);
            }
        }
    }
}
