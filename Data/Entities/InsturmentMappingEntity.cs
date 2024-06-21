namespace WebApplication1.Data.Entities
{
    public class InsturmentMappingEntity
    {
        public Guid InstrumentId { get; set; }
        public InstrumentsEntity Instrument { get; set; }
        public string MappingType { get; set; }
        public string Symbol { get; set; }
        public string Exhange { get; set; }
        public double DefaultOrderSize { get; set; }
    }
}
