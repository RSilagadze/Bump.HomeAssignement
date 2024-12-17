using Domain.Entities;
using Domain.Ports;

namespace Application.Clients
{
    public sealed class ClientHandler(IClientRepository repository)
    {
        public async Task<Client> CreateClientAsync(CreateClientCommand command, CancellationToken cancellationToken = default)
        {
            var cl = new Client(command.Name);
            await repository.SaveAsync(cl, cancellationToken);

            return cl;
        }
    }
}
