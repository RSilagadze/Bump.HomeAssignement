using Domain.Entities;
using Domain.Ports;

namespace Application.Appointments
{
    public sealed class AppointmentHandler(
        IAppointmentRepository appointmentRepository,
        IPatientRepository patientRepository)
    {

        public async Task<Appointment> ScheduleAppointmentAsync(ScheduleAppointmentCommand command, CancellationToken cancellationToken = default)
        {
            var patient = await patientRepository.GetAsync(command.PatientId, cancellationToken);
            
            if (patient == null)
                throw new InvalidOperationException($"Cannot find patient with id {command.PatientId}");

            var slot = new AppointmentSlot(command.StartDateTime, command.EndDateTime);
            var availableSlots = await appointmentRepository.GetAvailableAppointmentsAsync(cancellationToken);

            if (!availableSlots.Any())
                throw new InvalidOperationException("No more available appointment slots!");
            if (!availableSlots.Any(x => slot.IsWithinLimit(x.ScheduleStartDateTime, x.ScheduleEndDateTime)))
                throw new InvalidOperationException("Appointment slot is out of limit of available slots!");

            var appointment = new Appointment(command.Title, patient, slot);
            await appointmentRepository.SaveAsync(appointment, cancellationToken);

            return appointment;
        }

        public async Task<Appointment> ReScheduleAppointmentAsync(ReScheduleAppointmentCommand command, CancellationToken cancellationToken = default)
        {
            var appointment = await appointmentRepository.GetAsync(command.Id, cancellationToken);

            if (appointment == null)
                throw new InvalidOperationException($"Cannot find appointment with id {command.Id}");

            var slot = new AppointmentSlot(command.StartDateTime, command.EndDateTime);
            var availableSlots = await appointmentRepository.GetAvailableAppointmentsAsync(cancellationToken);

            if (!availableSlots.Any())
                throw new InvalidOperationException("No more available appointment slots!");
            if (!availableSlots.Any(x => slot.IsWithinLimit(x.ScheduleStartDateTime, x.ScheduleEndDateTime)))
                throw new InvalidOperationException("Appointment slot is out of limit of available slots!");

            appointment.Reschedule(new AppointmentSlot(command.StartDateTime, command.EndDateTime));
            await appointmentRepository.SaveAsync(appointment, cancellationToken);

            return appointment;
        }


        public async Task<Appointment> CancelAppointmentAsync(CancelAppointmentCommand command, CancellationToken cancellationToken = default)
        {
            var appointment = await appointmentRepository.GetAsync(command.Id, cancellationToken);

            if (appointment == null)
                throw new InvalidOperationException($"Cannot find appointment with id {command.Id}");
            
            appointment.Cancel();
            await appointmentRepository.SaveAsync(appointment, cancellationToken);

            return appointment;
        }



        public async Task<IEnumerable<AppointmentSlot>> GetAvailableAppointmentSlots(CancellationToken cancellationToken = default)
        {
            var slots = await appointmentRepository.GetAvailableAppointmentsAsync(cancellationToken);
            return slots;
        }
    }
}
