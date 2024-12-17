namespace Infrastructure.DTOs
{
    public sealed class PatientDbDTO
    {
        public long Id { get; internal set; }
        public string Name { get; internal set; }
        public long ClientId { get; internal set; }
    }
}
