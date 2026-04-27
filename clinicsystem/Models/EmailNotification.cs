namespace clinicsystem.Models
{
    public class EmailNotification
    {
        public int NotificationId { get; set; }

        public int ReservationId { get; set; }
        public int RecipientUserId { get; set; }

        public string Channel { get; set; }
        public string Status { get; set; }

        public DateTime SentAt { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Reservation Reservation { get; set; }
    }
}
