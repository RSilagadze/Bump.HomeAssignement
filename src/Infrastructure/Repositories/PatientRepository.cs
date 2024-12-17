using Domain.Ports;
using Domain.Entities;
using Infrastructure.DTOs;

namespace Infrastructure.Repositories
{
    public sealed class PatientRepository(Func<ICollection<PatientDbDTO>> patientsProviderFunc) : IPatientRepository
    {
        public async Task<Patient?> GetAsync(long id, CancellationToken cancellation = default)
        {
            var patientsSet = patientsProviderFunc();
            var patientDb = patientsSet.FirstOrDefault(x => x.Id == id);
            
            return patientDb == null ? 
                null : 
                new Patient(patientDb.Id, patientDb.Name);
        }
    }
}
