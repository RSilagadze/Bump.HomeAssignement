using Domain.Ports;
using Domain.Entities;
using Infrastructure.DTOs;

namespace Infrastructure.Repositories
{
    public sealed class ClientRepository(
        Func<ICollection<ClientDbDTO>> clientsProviderFunc,
        Func<ICollection<PatientDbDTO>> patientsProviderFunc)
        : IClientRepository
    {
        public Task SaveAsync(Client client, CancellationToken cancellation = default)
        {
            var clientSet = clientsProviderFunc();
            var patientsSet = patientsProviderFunc();
            
            if (client.Id <= 0)
            {
                var clientsGeneratedId = clientSet.Any() ? clientSet.Max(x => x.Id) + 1 : 1;
                var dbDto = new ClientDbDTO
                {
                    Id = clientsGeneratedId,
                    Name = client.Name
                };
                clientSet.Add(dbDto);
                client.Id = dbDto.Id;
                return Task.CompletedTask;
            }

            var foundClientInDb = clientSet.FirstOrDefault(x => x.Id == client.Id);
            if (foundClientInDb == null)
                return Task.CompletedTask;

            var patientsMaxId = patientsSet.Any() ? patientsSet.Max(x => x.Id) : 0;
            foreach (var clientNewAttachedPatient in client.NewAttachedPatients)
            {
                var patientDb = new PatientDbDTO
                {
                    ClientId = client.Id,
                    Name = clientNewAttachedPatient.Name,
                    Id = patientsMaxId += 1
                };
                patientsSet.Add(patientDb);
                clientNewAttachedPatient.Id = patientDb.Id;
            }

            return Task.CompletedTask;
        }

        public async Task<Client?> GetAsync(long id, CancellationToken cancellation = default)
        {
            var clientSet = clientsProviderFunc();
            var clientDb = clientSet.FirstOrDefault(x => x.Id == id);
            if (clientDb == null)
                return null;

            var patientsSet = patientsProviderFunc();
            var patients = new List<Patient>();
            patientsSet
                .Where(x => x.ClientId == id)
                .ToList()
                .ForEach(x =>
                {
                    patients.Add(new Patient(x.Id, x.Name));
                });

            return new Client(clientDb.Id, clientDb.Name, patients);
        }
    }
}
