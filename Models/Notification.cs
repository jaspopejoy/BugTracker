using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Net.Sockets;

namespace BugTracker.Models
{
    public class Notification
    {
        //Primary Key
        public int Id { get; set; }

        [DisplayName("Ticket")]
        public int TicketId { get; set; }

        [Required]
        [DisplayName("Title")]
        public string? Title { get; set; }

        [Required]
        [DisplayName("Message")]
        public string? Message { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Date")]
        public DateTimeOffset Created { get; set; }

        [Required]
        [DisplayName("Recipient")]
        public string? RecipientId { get; set; }

        [Required]
        [DisplayName("Sender")]
        public string? SenderId { get; set; }

        [DisplayName("Has Been Viewed")]
        public bool Viewed { get; set; }

        //Navigation Properties
        public virtual Ticket? Ticket { get; set; }
        public virtual BTUser? Recipient { get; set; }
        public virtual BTUser? Sender { get; set; }
    }
}
