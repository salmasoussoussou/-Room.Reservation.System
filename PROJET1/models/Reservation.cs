using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace projet1.models
         {
           public class Reservation
          {
          public int Id { get; set; }
          public int RoomId { get; set; }
          public DateTime StartTime { get; set; }
         public DateTime EndTime { get; set; }

         //Navigation property
       [ForeignKey("RoomId")]
        [JsonIgnore]
        [IgnoreDataMember]
        public Room Room { get; set; }
   }
}
