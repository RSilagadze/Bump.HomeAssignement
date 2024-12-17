namespace Domain.Entities
{
    //Simplified for demo
    public sealed class Patient
    {
        public long Id { get; internal set; }
        public string Name { get; }

        public Patient(string name)
        {
            Name = name;
            Id = 0;
        }

        public Patient(long id, string name)
        {
            Name = name;
            Id = id;
        }
    }
}
