namespace Bme.Aut.Logistics.Model
{
    public class Compliance
    {
        public long Id { get; set; }
        public string InvoiceID { get; set; }
        public bool IsDone { get; set; }

        public Section Section { get; set; }
        public Partner Partner { get; set; }    
    }
}
