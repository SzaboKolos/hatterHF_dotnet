using Bme.Aut.Logistics.Model;
using Bme.Aut.Logistics.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Bme.Aut.Logistics.Test
{
    [TestClass]
    public class F4AddressServiceIT
    {
        private readonly List<Address> otherAddresses = new List<Address>()
        {
            new Address(50, 19, "HU", "Other city", "1111", "street1", "2"),
            new Address(52, 13, "DE", "Berlin", "1111", "other street", "16"),
            new Address(52, 13, "DE", "Berlin", "1112", "old street", "15"),
            new Address(52, 13, "US", "Berlin", "1111", "old street", "15"),
        };

        private readonly List<Address> budapestAddresses = new List<Address>()
        {
            new Address(47.29, 19.04, "HU", "Budapest", "1111", "street1", "2"),
            new Address(47.30, 19.06, "HU", "budapest", "1111", "street1", "2"),
            new Address(47.31, 19.05, "HU", "BUDAPEST", "1111", "street1", "2"),
        };

        private List<Address> addressesInside = new List<Address>()
        {
            new Address(47.31, 17.05, "HU", "Other city 2", "1111", "street1", "2"),
            new Address(48.31, 16.05, "HU", "Other city 3", "1111", "street1", "2"),
        };

        private List<Address> addressesToRename = new List<Address>()
        {
            new Address(52, 13, "DE", "Berlin", "1111", "old street", "15"),
            new Address(52, 13, "DE", "Berlin", "1111", "old street", "16"),
        };

        [TestMethod]
        public void F4GetAddressesByCity()
        {
            using (var dbConn = TestDbHelper.CreateConnection())
            {
                var dbContext = TestDbHelper.CreateDbContext(dbConn);
                dbContext.Addresses.AddRange(budapestAddresses);
                dbContext.Addresses.AddRange(otherAddresses);
                dbContext.SaveChanges();

                var service = new AddressService(dbContext);

                var actual = service.FindByCityIgnoreCase("budapest");
                CollectionAssert.AreEquivalent(budapestAddresses, actual);

                actual = service.FindByCityIgnoreCase("bud");
                Assert.AreEqual(0, actual.Count);
            }
        }

        [TestMethod]
        public void F4GetAddressesInside()
        {
            using (var dbConn = TestDbHelper.CreateConnection())
            {
                var dbContext = TestDbHelper.CreateDbContext(dbConn);
                dbContext.Addresses.AddRange(addressesInside);
                dbContext.Addresses.AddRange(otherAddresses);
                dbContext.SaveChanges();

                var service = new AddressService(dbContext);

                var foundAddressesInside = service.FindByGeoLatBetweenAndGeoLngBetween(47, 49, 16, 18);
                CollectionAssert.AreEquivalent(addressesInside, foundAddressesInside);
            }
        }

        [TestMethod]
        public void F4RenameStreet()
        {
            using (var dbConn = TestDbHelper.CreateConnection())
            {
                var dbContext = TestDbHelper.CreateDbContext(dbConn);
                dbContext.Addresses.AddRange(addressesToRename);
                dbContext.Addresses.AddRange(otherAddresses);
                dbContext.SaveChanges();

                var service = new AddressService(dbContext);

                var newStreet = "renamed street";
                service.RenameStreet("DE", "1111", "old street", newStreet);

                var actual = dbContext.Addresses.Where(a => a.Street == newStreet).Select(a => a.Id).ToList();
                CollectionAssert.AreEquivalent(addressesToRename.Select(a => a.Id).ToList(), actual);
            }
        }
    }

}
