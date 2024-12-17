namespace Domain.Entities
{
    //Very simple aggregate
    public sealed class Appointment
    {
        public Appointment(string title, Patient patient, DateTime scheduleStartDateTime, DateTime scheduleEndDateTime)
        {
            Title = title;
            Patient = patient;
            ScheduleStartDateTime = scheduleStartDateTime;
            ScheduleEndDateTime = scheduleEndDateTime;

            if (DurationInMinutes() <= 0)
                throw new InvalidOperationException("Appointment duration cannot be 0 below or 0 minutes!");
        }

        public Appointment(long id, string title, Patient patient, DateTime scheduleStartDateTime, DateTime scheduleEndDateTime)
        {
            Title = title;
            Patient = patient;
            ScheduleStartDateTime = scheduleStartDateTime;
            ScheduleEndDateTime = scheduleEndDateTime;
            Id = id;

            if (DurationInMinutes() <= 0)
                throw new InvalidOperationException("Appointment duration cannot be 0 below or 0 minutes!");
        }

        public long Id { get; internal set; }         
        public string Title { get; }
        public Patient Patient { get; }
        public bool IsCancelled() => CancelDateTime != null;
        public long DurationInMinutes() => (int)(ScheduleEndDateTime - ScheduleStartDateTime).TotalMinutes;

        public DateTime ScheduleStartDateTime { get; private set; }
        public DateTime ScheduleEndDateTime { get; private set; }
        public DateTime? CancelDateTime { get; private set; }

        public void Reschedule(DateTime newStartDateTime, DateTime newEndDateTime)
        {
            if (newEndDateTime <= newStartDateTime)
                throw new InvalidOperationException("End date time cannot be earlier or same as start date!");
            ScheduleStartDateTime = newStartDateTime;
            ScheduleEndDateTime = newEndDateTime;
        }

        public void Cancel()
        {
            CancelDateTime = DateTime.Now;
        }

    }
}
