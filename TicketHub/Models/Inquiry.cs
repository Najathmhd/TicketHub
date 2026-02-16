using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TicketHub.Models;

[Table("Inquiry")]
public partial class Inquiry
{
    [Key]
    [Column("InquiryID")]
    public int InquiryId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? GuestEmail { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string? Message { get; set; }

    public DateOnly? InquiryDate { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? Status { get; set; }

    [Column("MemberID")]
    public int MemberId { get; set; }

    [ForeignKey("MemberId")]
    [InverseProperty("Inquiries")]
    public virtual Member Member { get; set; } = null!;
}
