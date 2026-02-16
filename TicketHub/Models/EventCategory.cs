using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TicketHub.Models;

[Table("EventCategory")]
public partial class EventCategory
{
    [Key]
    [Column("CategoryID")]
    public int CategoryId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? CategoryName { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Description { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
