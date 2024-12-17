using Domain.Entities;

namespace Domain.Ports
{
    //Combined in one file for simplicity 
    public interface IAppointmentRepository
    {
        Task SaveAsync(Appointment appointment, CancellationToken cancellation = default);
        Task<Appointment?> GetAsync(long id, CancellationToken cancellation = default);
        Task<IEnumerable<AppointmentSlot>> GetAvailableAppointmentsAsync(CancellationToken cancellation = default);
    }

    public interface IPatientRepository
    {
        Task<Patient?> GetAsync(long id, CancellationToken cancellation = default);
    }

    public interface IClientRepository
    {
        Task SaveAsync(Client client, CancellationToken cancellation = default);
        Task<Client?> GetAsync(long id, CancellationToken cancellation = default);
    }
}
