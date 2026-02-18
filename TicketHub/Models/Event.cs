using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TicketHub.Models;

[Table("Event")]
public partial class Event
{
    [Key]
    [Column("EventID")]
    public int EventId { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string? Title { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string? Description { get; set; }

    public DateOnly? EventDate { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? EventTime { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? EventStatus { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? ImageUrl { get; set; }

    [Column("CategoryID")]
    public int CategoryId { get; set; }

    [Column("VenueID")]
    public int VenueId { get; set; }

    [Column("OrganizerID")]
    public int? OrganizerId { get; set; }

    [ValidateNever]
    [ForeignKey("OrganizerId")]
    [InverseProperty("OrganizedEvents")] 
    public virtual Member? Organizer { get; set; }

    [ValidateNever]
    [InverseProperty("Event")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [ValidateNever]
    [ForeignKey("CategoryId")]
    [InverseProperty("Events")]
    public virtual EventCategory Category { get; set; } = null!;

    [ValidateNever]
    [InverseProperty("Event")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [ValidateNever]
    [InverseProperty("Event")]
    public virtual ICollection<TicketType> TicketTypes { get; set; } = new List<TicketType>();

    [ValidateNever]
    [ForeignKey("VenueId")]
    [InverseProperty("Events")]
    public virtual Venue Venue { get; set; } = null!;
}
