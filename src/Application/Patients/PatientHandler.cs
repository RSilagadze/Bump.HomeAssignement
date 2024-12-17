using Domain.Entities;
using Domain.Ports;

namespace Application.Patients
{
    //Very simplified
    public sealed class PatientHandler
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IClientRepository _clientRepository;

        public PatientHandler(IPatientRepository patientRepository, IClientRepository clientRepository)
        {
            _patientRepository = patientRepository;
            _clientRepository = clientRepository;
        }

        public async Task<Patient> CreatePatientAsync(CreatePatientCommand command, CancellationToken cancellation = default)
        {
            var client = await _clientRepository.GetAsync(command.ClientId, cancellation);
            
            //Throw business exception here e.g. ClientNotFoundException
            if (client == null)
                throw new InvalidOperationException($"Cannot find client with id {command.ClientId}");

            var patient = new Patient(command.Name);
            client.AttachPatient(patient);
            await _clientRepository.SaveAsync(client, cancellation);
            
            return patient;
        }
    }
}
