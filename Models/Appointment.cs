namespace Reservation.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime? ReservationTime { get; set; }
        public DateTime? ConfirmationTime { get; set; }
        public bool IsReserved { get; set; }
        public bool IsConfirmed {  get; set; }
    }

}
