namespace Domain.Entities
{
    //Simplified for demo
    public sealed class Client
    {
        public Client(string name)
        {
            Name = name;
            Id = 0;
            _patients = [];
            _newAttachedPatients = [];
        }
        public Client(long id, string name, ICollection<Patient> patients)
        {
            Name = name;
            Id = id;
            _patients = patients;
            _newAttachedPatients = [];
        }

        private readonly ICollection<Patient> _patients;
        private readonly ICollection<Patient> _newAttachedPatients;
        public IEnumerable<Patient> Patients => _patients;
        public IEnumerable<Patient> NewAttachedPatients => _newAttachedPatients;
        public string Name { get; }
        public long Id { get; internal set; }

        public void AttachPatient(Patient patient)
        {
            if (_newAttachedPatients.Any(x => x.Id == patient.Id) || _patients.Any(x => x.Id == patient.Id))
                throw new InvalidOperationException($"The patient with id {patient.Id} already attached for this client!");

            _newAttachedPatients.Add(patient);
            _patients.Add(patient);
        }

    }
}
