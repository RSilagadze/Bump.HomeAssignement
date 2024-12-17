namespace Infrastructure.DTOs
{
    public sealed class AppointmentDbDTO
    {
        public long Id { get; internal set; }
        public string Title { get; internal set; }
        public long PatientId { get; internal set; }

        public DateTime ScheduleStartDateTime { get; internal set; }
        public DateTime ScheduleEndDateTime { get; internal set; }
        public DateTime? CancelDateTime { get; internal set; }
    }
}
