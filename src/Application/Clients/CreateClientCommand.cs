using System.Text.Json.Serialization;

namespace Application.Clients
{
    public sealed class CreateClientCommand
    {
        [JsonConstructor]
        public CreateClientCommand(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
