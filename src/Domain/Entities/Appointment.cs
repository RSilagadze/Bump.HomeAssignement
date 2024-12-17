namespace Domain.Entities
{
    //Very simple aggregate
    public sealed class Appointment
    {
        public Appointment(string title, Patient patient, AppointmentSlot appointmentSlot)
        {
            Title = title;
            Patient = patient;
            AppointmentSlot = appointmentSlot;
        }

        public Appointment(long id, string title, Patient patient, AppointmentSlot appointmentSlot, DateTime? cancelDateTime)
        {
            Title = title;
            Patient = patient;
            Id = id;
            AppointmentSlot = appointmentSlot;
            CancelDateTime = cancelDateTime;
        }

        public long Id { get; internal set; }         
        public string Title { get; }
        public Patient Patient { get; }
        public bool IsCancelled() => CancelDateTime != null;
        public AppointmentSlot AppointmentSlot { get; private set; }

        public DateTime? CancelDateTime { get; private set; }

        public void Reschedule(AppointmentSlot newAppointmentSlot)
        {
            if (IsCancelled())
                throw new InvalidOperationException($"Appointment {Id} already cancelled!");
            AppointmentSlot = newAppointmentSlot;
        }

        public void Cancel()
        {
            if (IsCancelled())
                throw new InvalidOperationException($"Appointment {Id} already cancelled!");
            CancelDateTime = DateTime.Now;
        }

    }
}
