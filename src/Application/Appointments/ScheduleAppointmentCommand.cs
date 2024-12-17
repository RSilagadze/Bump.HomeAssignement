using System.Text.Json.Serialization;

namespace Application.Appointments
{
    public sealed class ScheduleAppointmentCommand
    {
        [JsonConstructor]
        public ScheduleAppointmentCommand(string title,
            DateTime startDateTime,
            DateTime endDateTime,
            long patientId)
        {
            PatientId = patientId;
            Title = title;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
        }

        public long PatientId { get; }

        public string Title { get; }
        public DateTime StartDateTime { get; }
        public DateTime EndDateTime { get; }
    }
}
