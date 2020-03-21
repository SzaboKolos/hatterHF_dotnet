using Bme.Aut.Logistics.Dal;
using Bme.Aut.Logistics.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bme.Aut.Logistics.Service
{
    public class AddressService
    {
        private readonly LogisticsDbContext dbContext;

        public AddressService(LogisticsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // TODO: Megvalósítani az 4. a. feladat szerint
        // Tipp 1: a.Equals(b, StringComparison.InvariantCultureIgnoreCase)
        // Tipp 2: Sajnos nem tudja leforditani az EF a lekerdezest. Muszaj memoriaban csinalni az osszehasonlitast: dbContext.Addresses.ToList().Where(...
        public List<Address> FindByCityIgnoreCase(string city)
        {
            throw new NotImplementedException();
        }

        // TODO: Megvalósítani az 4. b. feladat szerint
        public List<Address> FindByGeoLatBetweenAndGeoLngBetween(double minLat, double maxLat, double minLng, double maxLng)
        {
            throw new NotImplementedException();
        }

        // TODO: Megvalósítani a 4. c. feladat szerint
        public void RenameStreet(string country, string zipCode, string oldStreet, string newStreet)
        {
            throw new NotImplementedException();
        }
    }
}
