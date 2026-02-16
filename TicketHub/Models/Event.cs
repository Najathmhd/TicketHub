using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

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

    [Column("CategoryID")]
    public int CategoryId { get; set; }

    [Column("VenueID")]
    public int VenueId { get; set; }

    [InverseProperty("Event")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [ForeignKey("CategoryId")]
    [InverseProperty("Events")]
    public virtual EventCategory Category { get; set; } = null!;

    [InverseProperty("Event")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [InverseProperty("Event")]
    public virtual ICollection<TicketType> TicketTypes { get; set; } = new List<TicketType>();

    [ForeignKey("VenueId")]
    [InverseProperty("Events")]
    public virtual Venue Venue { get; set; } = null!;
}
