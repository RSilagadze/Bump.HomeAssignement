using System.Text.Json.Serialization;

namespace Application.Appointments
{
    public sealed class ReScheduleAppointmentCommand
    {
        [JsonConstructor]
        public ReScheduleAppointmentCommand(DateTime startDateTime,
            DateTime endDateTime,
            long id)
        {
            Id = id;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
        }

        public long Id { get; }

        public DateTime StartDateTime { get; }
        public DateTime EndDateTime { get; }
    }
}
