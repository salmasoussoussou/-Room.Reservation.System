namespace projet1.DTO
{
    public class dtoreservation
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public dtoroom Room { get; set; }
    }
}
