using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Bme.Aut.Logistics.Test
{
    [TestClass]
    public class F2ActionEntityTest
    {
        [TestMethod]
        public void F2ActionEntityDefined()
        {
            using (var dbConn = TestDbHelper.CreateConnection())
            {
                var dbContext = TestDbHelper.CreateDbContext(dbConn);

                var actionEntityType = dbContext.Model.FindEntityType(typeof(Model.Action));
                Assert.IsNotNull(actionEntityType, "F2 / Action entitas nem ismert a DbContext-ben");

                var primaryKey = actionEntityType.FindPrimaryKey();
                Assert.IsNotNull(primaryKey, "F2 / Action entitas nem tartalmaz kulcsot");

                var dateTimeProperties = actionEntityType.GetProperties().Count(p => p.ClrType == typeof(DateTime?));
                Assert.AreEqual(1, dateTimeProperties, "F2 / Action entitas elvegzes ideje hianyzik (vagy rossz a tipusa)");

                var enumProperties = actionEntityType.GetProperties().Count(p => p.ClrType.IsEnum);
                Assert.AreEqual(1, enumProperties, "F2 / Action entitas tevekenyseg tipusa hianyzik (vagy rossz a tipusa)");
            }
        }

        [TestMethod]
        public void F2ActionEntityReferencedByMilestone()
        {
            using (var dbConn = TestDbHelper.CreateConnection())
            {
                var dbContext = TestDbHelper.CreateDbContext(dbConn);

                var actionEntityType = dbContext.Model.FindEntityType(typeof(Model.Action));
                Assert.IsNotNull(actionEntityType, "F2 / Action entitas nem ismert a DbContext-ben");

                var foreignKeys = actionEntityType.GetForeignKeys().Where(fk => fk.PrincipalEntityType?.ClrType == typeof(Model.Milestone)).ToList();
                Assert.AreEqual(1, foreignKeys.Count, "F2 / Action entitas nem tartalmaz Milestone-ra mutato kulso kulcsot (vagy a kapcsolat nem ketiranyu)");
                Assert.IsNotNull(foreignKeys.Single().DependentToPrincipal?.ForeignKey, "F2 / Action entitas nem tartalmaz Milestone-ra mutato navigation property-t");
                Assert.IsNotNull(foreignKeys.Single().PrincipalToDependent, "F2 / Milestone entitas nem tartalmaz Action-re mutato navigation property-t");
            }
        }
    }

}
