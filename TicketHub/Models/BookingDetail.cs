using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TicketHub.Models;

[Table("BookingDetail")]
public partial class BookingDetail
{
    [Key]
    [Column("BookingDetailID")]
    public int BookingDetailId { get; set; }

    public int? Quantity { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? SubTotal { get; set; }

    [Column("BookingID")]
    public int BookingId { get; set; }

    [Column("TicketTypeID")]
    public int TicketTypeId { get; set; }

    [ValidateNever]
    [ForeignKey("BookingId")]
    [InverseProperty("BookingDetails")]
    public virtual Booking Booking { get; set; } = null!;

    [ValidateNever]
    [ForeignKey("TicketTypeId")]
    [InverseProperty("BookingDetails")]
    public virtual TicketType TicketType { get; set; } = null!;
}
