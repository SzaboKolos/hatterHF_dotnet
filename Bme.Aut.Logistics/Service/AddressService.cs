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
            return dbContext.Addresses.ToList().Where(c => c.City.Equals(city, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        // TODO: Megvalósítani az 4. b. feladat szerint
        public List<Address> FindByGeoLatBetweenAndGeoLngBetween(double minLat, double maxLat, double minLng, double maxLng)
        {
            return dbContext.Addresses.ToList().Where(c => c.GeoLatitude <= maxLat
                                                        && c.GeoLatitude > minLat
                                                        && c.GeoLongitude <= maxLng
                                                        && c.GeoLongitude > minLng).ToList();
        }

        // TODO: Megvalósítani a 4. c. feladat szerint
        public void RenameStreet(string country, string zipCode, string oldStreet, string newStreet)
        {
             var addr = dbContext.Addresses.ToList().Where(x => x.Country.Equals(country, StringComparison.InvariantCultureIgnoreCase)
                                                             && x.ZipCode.Equals(zipCode, StringComparison.InvariantCultureIgnoreCase)
                                                             && x.Street.Equals(oldStreet, StringComparison.InvariantCultureIgnoreCase)).ToList();
        
            foreach (var a in addr)
            {
                a.Street = newStreet;
                dbContext.SaveChanges();
            }
            
        }
    }
}