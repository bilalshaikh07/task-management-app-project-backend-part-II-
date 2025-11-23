using System;
using System.Collections.Generic;

namespace TaskMgmAPI.Models;

public partial class Task
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime? DueDate { get; set; }

    public int UserId { get; set; }
}
