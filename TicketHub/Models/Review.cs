using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TicketHub.Models;

[Table("Review")]
[Index("MemberId", "EventId", Name = "UQ_Review", IsUnique = true)]
public partial class Review
{
    [Key]
    [Column("ReviewID")]
    public int ReviewId { get; set; }

    public int? Rating { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string? Comment { get; set; }

    public DateOnly? ReviewDate { get; set; }

    [Column("MemberID")]
    public int MemberId { get; set; }

    [Column("EventID")]
    public int EventId { get; set; }

    [ValidateNever]
    [ForeignKey("EventId")]
    [InverseProperty("Reviews")]
    public virtual Event Event { get; set; } = null!;

    [ValidateNever]
    [ForeignKey("MemberId")]
    [InverseProperty("Reviews")]
    public virtual Member Member { get; set; } = null!;
}
