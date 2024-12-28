using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace projet1.models
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public bool IsAvailable { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public List<Reservation> reservations { get; set; }
    }
}

