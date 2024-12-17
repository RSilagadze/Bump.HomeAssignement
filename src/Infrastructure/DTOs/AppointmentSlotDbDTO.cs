namespace Infrastructure.DTOs
{
    public sealed class AppointmentSlotDbDTO
    {
        public AppointmentSlotDbDTO(DateTime startDateTime, DateTime endDatetime)
        {
            StartDateTime = startDateTime;
            EndDateTime = endDatetime;
        }
        public DateTime StartDateTime { get; }
        public DateTime EndDateTime { get; }

        public override int GetHashCode()
        {
            return HashCode.Combine(StartDateTime, EndDateTime);
        }

        public override bool Equals(object? obj)
        {
            return (obj is AppointmentSlotDbDTO appointmentDto) &&
                   (appointmentDto.StartDateTime >= StartDateTime && appointmentDto.EndDateTime <= EndDateTime);
        }
    }

}
