using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Data.Entities
{
    public class BaseEntity
    {
        
        [Key]
        public Guid InstrumentId { get; set; }
        public InstrumentsEntity Instrument { get; set; }
        public string Provider { get; set; }
        public string Type { get; set; }
    }
}
