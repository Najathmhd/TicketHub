using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TicketHub.Models;

[Table("Venue")]
public partial class Venue
{
    [Key]
    [Column("VenueID")]
    public int VenueId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string VenueName { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string? Location { get; set; }

    public int? TotalCapacity { get; set; }

    [InverseProperty("Venue")]
    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
