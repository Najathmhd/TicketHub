using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TicketHub.Models.ViewModels
{
    public class BookingCreateViewModel
    {
        public int EventId { get; set; }

        [ValidateNever]
        public string EventTitle { get; set; }

        [ValidateNever]
        public DateOnly? EventDate { get; set; }

        [ValidateNever]
        public string VenueName { get; set; }

        [Required]
        [Display(Name = "Ticket Type")]
        public int SelectedTicketTypeId { get; set; }

        [Required]
        [Range(1, 10, ErrorMessage = "You must book at least 1 ticket and maximum 10.")]
        public int Quantity { get; set; }

        [ValidateNever]
        public IEnumerable<TicketType> TicketTypes { get; set; }
    }
}
