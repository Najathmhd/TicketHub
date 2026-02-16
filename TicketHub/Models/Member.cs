using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TicketHub.Models;

[Table("Member")]
[Index("Email", Name = "UQ__Member__A9D1053458292442", IsUnique = true)]
public partial class Member
{
    [Key]
    [Column("MemberID")]
    public int MemberId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string FullName { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string PasswordHash { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string? PhoneNumber { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? PreferredCategory { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? Role { get; set; }

    public DateOnly? RegistrationDate { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? Status { get; set; }

    [InverseProperty("Member")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [InverseProperty("Member")]
    public virtual ICollection<Inquiry> Inquiries { get; set; } = new List<Inquiry>();

    [InverseProperty("Member")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
