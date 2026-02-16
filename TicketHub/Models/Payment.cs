using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TicketHub.Models;

[Table("Payment")]
[Index("BookingId", Name = "UQ__Payment__73951ACCF399DFE1", IsUnique = true)]
public partial class Payment
{
    [Key]
    [Column("PaymentID")]
    public int PaymentId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? PaymentMethod { get; set; }

    public DateOnly? PaymentDate { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? Amount { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? PaymentStatus { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? TransactionReference { get; set; }

    [Column("BookingID")]
    public int BookingId { get; set; }

    [ValidateNever]
    [ForeignKey("BookingId")]
    [InverseProperty("Payment")]
    public virtual Booking Booking { get; set; } = null!;
}
