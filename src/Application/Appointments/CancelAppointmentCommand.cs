using System.Text.Json.Serialization;

namespace Application.Appointments
{
    public sealed class CancelAppointmentCommand
    {
        [JsonConstructor]
        public CancelAppointmentCommand(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }
}
