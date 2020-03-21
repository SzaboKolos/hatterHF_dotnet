using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Bme.Aut.Logistics.Test
{
    [TestClass]
    public class F1PartnerEntityTest
    {
        [TestMethod]
        public void F1PartnerEntityDefined()
        {
            using (var dbConn = TestDbHelper.CreateConnection())
            {
                var dbContext = TestDbHelper.CreateDbContext(dbConn);

                var partnerEntityType = dbContext.Model.FindEntityType(typeof(Model.Partner));
                Assert.IsNotNull(partnerEntityType, "F1 / Partner entitas nem ismert a DbContext-ben");

                var primaryKey = partnerEntityType.FindPrimaryKey();
                Assert.IsNotNull(primaryKey, "F1 / Partner entitas nem tartalmaz kulcsot");

                var stringProperties = partnerEntityType.GetProperties().Count(p => p.ClrType == typeof(string));
                Assert.AreEqual(2, stringProperties, "F1 / Partner entitas nev/email hianyzik (vagy felesleges szoveges adatok vannak definialva)");
            }
        }

        [TestMethod]
        public void F1PartnerEntityHasAddress()
        {
            using (var dbConn = TestDbHelper.CreateConnection())
            {
                var dbContext = TestDbHelper.CreateDbContext(dbConn);

                var partnerEntityType = dbContext.Model.FindEntityType(typeof(Model.Partner));
                Assert.IsNotNull(partnerEntityType, "F1 / Partner entitas nem ismert a DbContext-ben");

                var foreignKeys = partnerEntityType.GetForeignKeys().Where(fk => fk.PrincipalKey?.DeclaringEntityType?.ClrType == typeof(Model.Address)).ToList();
                Assert.AreEqual(1, foreignKeys.Count, "F1 / Partner entitas nem tartalmaz Address-re mutato kulso kulcsot (vagy tobb, mint 1 van)");
                Assert.IsNotNull(foreignKeys.Single().DependentToPrincipal?.ForeignKey, "F1 / Partner entitas nem tartalmaz Address-re mutato navigation property-t");
            }
        }
    }

}
