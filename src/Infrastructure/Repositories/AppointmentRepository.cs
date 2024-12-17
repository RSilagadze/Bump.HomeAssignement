using Domain.Ports;
using Domain.Entities;
using Infrastructure.DTOs;

namespace Infrastructure.Repositories
{
    public sealed class AppointmentRepository(Func<ICollection<PatientDbDTO>> patientsProviderFunc, 
        Func<ICollection<AppointmentDbDTO>> appointmentsProviderFunc,
        Func<ICollection<AppointmentSlotDbDTO>> appointmentsSlotProviderFunc) : IAppointmentRepository
    {
        public Task SaveAsync(Appointment appointment, CancellationToken cancellation = default)
        {
            var appointmentsSet = appointmentsProviderFunc();
            var appointmentSlotsSet = appointmentsSlotProviderFunc();

            if (appointment.Id <= 0)
            {
                var appointmentsGeneratedId = appointmentsSet.Any() ? appointmentsSet.Max(x => x.Id) + 1 : 1;
                var dbDto = new AppointmentDbDTO
                {
                    Id = appointmentsGeneratedId,
                    PatientId = appointment.Patient.Id,
                    CancelDateTime = appointment.CancelDateTime,
                    ScheduleEndDateTime = appointment.AppointmentSlot.ScheduleEndDateTime,
                    ScheduleStartDateTime = appointment.AppointmentSlot.ScheduleStartDateTime,
                    Title = appointment.Title
                };
                appointmentsSet.Add(dbDto);
                appointment.Id = dbDto.Id;
                appointmentSlotsSet.Remove(new AppointmentSlotDbDTO(appointment.AppointmentSlot.ScheduleStartDateTime, appointment.AppointmentSlot.ScheduleEndDateTime));

                return Task.CompletedTask;
            }

            var appointmentDb = appointmentsSet.FirstOrDefault(x => x.Id == appointment.Id);

            if (appointmentDb == null)
                throw new InvalidOperationException($"Appointment {appointment.Id} was not found in database while saving operation!");

            appointmentDb.ScheduleEndDateTime = appointment.AppointmentSlot.ScheduleEndDateTime;
            appointmentDb.ScheduleStartDateTime = appointment.AppointmentSlot.ScheduleStartDateTime;
            appointmentDb.CancelDateTime = appointment.CancelDateTime;
            appointmentDb.Title = appointment.Title;
            appointmentSlotsSet.Remove(new AppointmentSlotDbDTO(appointment.AppointmentSlot.ScheduleStartDateTime, appointment.AppointmentSlot.ScheduleEndDateTime));

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
                throw new InvalidOperationException($"Patient with id {appointmentDb.PatientId} does not exist in DB while retrieving appointment!");

            var patient = new Patient(patientDb.Id, patientDb.Name);
            return new Appointment(appointmentDb.Id, appointmentDb.Title, patient, new AppointmentSlot(appointmentDb.ScheduleStartDateTime, appointmentDb.ScheduleEndDateTime), appointmentDb.CancelDateTime);
        }

        public async Task<IEnumerable<AppointmentSlot>> GetAvailableAppointmentsAsync(CancellationToken cancellation = default)
        {
            var appointmentSlotsDb = appointmentsSlotProviderFunc();

            return appointmentSlotsDb
                .Select(slotDb => new AppointmentSlot(slotDb.StartDateTime, slotDb.EndDateTime))
                .ToList();
        }
    }
}
