namespace Bme.Aut.Logistics.Model
{
    public class Partner
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public Address Address { get; set; }
    }
}