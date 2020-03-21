using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Bme.Aut.Logistics.Test
{
    [TestClass]
    public class F3ComplianceEntityTest
    {
        [TestMethod]
        public void F3ComplianceEntityDefined()
        {
            using (var dbConn = TestDbHelper.CreateConnection())
            {
                var dbContext = TestDbHelper.CreateDbContext(dbConn);

                var actionEntityType = dbContext.Model.FindEntityType(typeof(Model.Compliance));
                Assert.IsNotNull(actionEntityType, "F3 / Compliance entitas nem ismert a DbContext-ben");

                var primaryKey = actionEntityType.FindPrimaryKey();
                Assert.IsNotNull(primaryKey, "F3 / Compliance entitas nem tartalmaz kulcsot");

                var stringProperties = actionEntityType.GetProperties().Count(p => p.ClrType == typeof(string));
                Assert.AreEqual(1, stringProperties, "F3 / Compliance entitas szamla azonositot nem tartalmaz (vagy rossz a tipusa)");

                var boolProperties = actionEntityType.GetProperties().Count(p => p.ClrType == typeof(bool));
                Assert.AreEqual(1, boolProperties, "F3 / Compliance entitas teljesitett-e mezot nem tartalmaz (vagy rossz a tipusa)");
            }
        }

        [TestMethod]
        public void F3ComplianceEntityReferencedByMilestone()
        {
            using (var dbConn = TestDbHelper.CreateConnection())
            {
                var dbContext = TestDbHelper.CreateDbContext(dbConn);

                var actionEntityType = dbContext.Model.FindEntityType(typeof(Model.Compliance));
                Assert.IsNotNull(actionEntityType, "F3 / Compliance entitas nem ismert a DbContext-ben");

                var foreignKeys = actionEntityType.GetForeignKeys().ToList();
                Assert.AreEqual(1, foreignKeys.Count(fk => fk.PrincipalEntityType?.ClrType == typeof(Model.Section)), "F3 / Compliance entitas nem tartalmaz Section-ra mutato kulso kulcsot");
                Assert.IsNotNull(foreignKeys.Single(fk => fk.PrincipalEntityType?.ClrType == typeof(Model.Section)).DependentToPrincipal?.ForeignKey, "F3 / Compliance entitas nem tartalmaz Section-re mutato navigation property-t");

                Assert.AreEqual(1, foreignKeys.Count(fk => fk.PrincipalEntityType?.ClrType == typeof(Model.Partner)), "F3 / Compliance entitas nem tartalmaz Partner-ra mutato kulso kulcsot");
                Assert.IsNotNull(foreignKeys.Single(fk => fk.PrincipalEntityType?.ClrType == typeof(Model.Partner)).DependentToPrincipal?.ForeignKey, "F3 / Compliance entitas nem tartalmaz Partner-re mutato navigation property-t");
            }
        }
    }

}
