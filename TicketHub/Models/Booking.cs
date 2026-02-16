using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TicketHub.Models;

[Table("Booking")]
public partial class Booking
{
    [Key]
    [Column("BookingID")]
    public int BookingId { get; set; }

    public DateOnly? BookingDate { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? TotalAmount { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? BookingStatus { get; set; }

    [Column("MemberID")]
    public int MemberId { get; set; }

    [Column("EventID")]
    public int EventId { get; set; }

    [ValidateNever]
    [InverseProperty("Booking")]
    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    [ValidateNever]
    [ForeignKey("EventId")]
    [InverseProperty("Bookings")]
    public virtual Event Event { get; set; } = null!;

    [ValidateNever]
    [ForeignKey("MemberId")]
    [InverseProperty("Bookings")]
    public virtual Member Member { get; set; } = null!;

    [ValidateNever]
    [InverseProperty("Booking")]
    public virtual Payment? Payment { get; set; }
}
