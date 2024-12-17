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

            var appointment = new Appointment(command.Title, patient, command.StartDateTime, command.EndDateTime);
            await appointmentRepository.SaveAsync(appointment, cancellationToken);

            return appointment;
        }

        public async Task<Appointment> ReScheduleAppointmentAsync(ReScheduleAppointmentCommand command, CancellationToken cancellationToken = default)
        {
            var appointment = await appointmentRepository.GetAsync(command.Id, cancellationToken);

            if (appointment == null)
                throw new InvalidOperationException($"Cannot find appointment with id {command.Id}");

            appointment.Reschedule(command.StartDateTime, command.EndDateTime);
            await appointmentRepository.SaveAsync(appointment, cancellationToken);

            return appointment;
        }


        public async Task<Appointment> CancelAppointmentAsync(CancelAppointmentCommand command, CancellationToken cancellationToken = default)
        {
            var appointment = await appointmentRepository.GetAsync(command.Id, cancellationToken);

            if (appointment == null)
                throw new InvalidOperationException($"Cannot find appointment with id {command.Id}");
            if (appointment.IsCancelled())
                throw new InvalidOperationException($"Appointment {command.Id} already cancelled!");

            appointment.Cancel();
            await appointmentRepository.SaveAsync(appointment, cancellationToken);

            return appointment;
        }
    }
}
