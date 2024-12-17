using Domain.Ports;
using Domain.Entities;
using Infrastructure.DTOs;

namespace Infrastructure.Repositories
{
    public sealed class AppointmentRepository(Func<ICollection<PatientDbDTO>> patientsProviderFunc, 
        Func<ICollection<AppointmentDbDTO>> appointmentsProviderFunc) : IAppointmentRepository
    {
        public Task SaveAsync(Appointment appointment, CancellationToken cancellation = default)
        {
            var appointmentsSet = appointmentsProviderFunc();

            if (appointment.Id <= 0)
            {
                var appointmentsGeneratedId = appointmentsSet.Any() ? appointmentsSet.Max(x => x.Id) + 1 : 1;
                var dbDto = new AppointmentDbDTO
                {
                    Id = appointmentsGeneratedId,
                    PatientId = appointment.Patient.Id,
                    CancelDateTime = appointment.CancelDateTime,
                    ScheduleEndDateTime = appointment.ScheduleEndDateTime,
                    ScheduleStartDateTime = appointment.ScheduleStartDateTime,
                    Title = appointment.Title
                };
                appointmentsSet.Add(dbDto);
                appointment.Id = dbDto.Id;
                return Task.CompletedTask;
            }

            var appointmentDb = appointmentsSet.FirstOrDefault(x => x.Id == appointment.Id);

            if (appointmentDb == null)
                return Task.CompletedTask;

            appointmentDb.ScheduleEndDateTime = appointment.ScheduleEndDateTime;
            appointmentDb.ScheduleStartDateTime = appointment.ScheduleStartDateTime;
            appointmentDb.CancelDateTime = appointment.CancelDateTime;
            appointmentDb.Title = appointment.Title;
 
            return Task.CompletedTask;
        }

        public async Task<Appointment?> GetAsync(long id, CancellationToken cancellation = default)
        {
            var appointmentsSet = appointmentsProviderFunc();
            var patientsSet = patientsProviderFunc();
            var appointmentDb = appointmentsSet.FirstOrDefault(x => x.Id == id);

            if (appointmentDb == null)
                return null;

            var patientDb = patientsSet.FirstOrDefault(x => x.Id == appointmentDb.PatientId);
            if (patientDb == null)
                throw new InvalidOperationException($"Patient with id {appointmentDb.PatientId} does not exist in DB!");

            var patient = new Patient(patientDb.Id, patientDb.Name);
            return new Appointment(appointmentDb.Id, appointmentDb.Title, patient, appointmentDb.ScheduleStartDateTime, appointmentDb.ScheduleEndDateTime);

        }

        public Task<IEnumerable<Appointment>> GetAvailableAppointmentsAsync(CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }
    }
}
