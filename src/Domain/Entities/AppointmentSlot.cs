namespace Domain.Entities
{
    public sealed class AppointmentSlot
    {
        public AppointmentSlot(DateTime scheduleStartDateTime, DateTime scheduleEndDateTime)
        {
            if (scheduleEndDateTime <= scheduleStartDateTime)
                throw new InvalidOperationException("Appointment start time cannot exceed or be equal to end time!");
           
            DurationInMinutes = (int)(scheduleEndDateTime - scheduleStartDateTime).TotalMinutes;

            if (DurationInMinutes < 1)
                throw new InvalidOperationException("Appointment duration cannot be less than minute!");

            ScheduleStartDateTime = scheduleStartDateTime;
            ScheduleEndDateTime = scheduleEndDateTime;
        }

        public long DurationInMinutes { get; }
        public DateTime ScheduleStartDateTime { get; }
        public DateTime ScheduleEndDateTime { get;}

        public bool IsWithinLimit(DateTime startDateTime, DateTime endDateTime)
        {
            return ScheduleStartDateTime >= startDateTime && ScheduleEndDateTime <= endDateTime;
        }
    }
}
