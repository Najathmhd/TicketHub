using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TicketHub.Models;

[Table("TicketType")]
public partial class TicketType
{
    [Key]
    [Column("TicketTypeID")]
    public int TicketTypeId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? TypeName { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? Price { get; set; }

    public int? SeatLimit { get; set; }

    public int? AvailableSeats { get; set; }

    [Column("EventID")]
    public int EventId { get; set; }

    [ValidateNever]
    [InverseProperty("TicketType")]
    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    [ValidateNever]
    [ForeignKey("EventId")]
    [InverseProperty("TicketTypes")]
    public virtual Event Event { get; set; } = null!;
}
