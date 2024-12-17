using System.Text.Json.Serialization;

namespace Application.Patients
{
    public sealed class CreatePatientCommand
    {
        [JsonConstructor]
        public CreatePatientCommand(string name, long clientId)
        {
            Name = name;
            ClientId = clientId;
        }

        public string Name { get; }
        public long ClientId { get; }
    }
}
