namespace Reservation.Models
{
    public class Provider
    {
        public int Id { get; set; }
        // In production system, we should store provider name as well.
        public string? Name { get; set; }
    }

}
