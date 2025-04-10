using System;
using System.ComponentModel.DataAnnotations;

namespace MVCCoreDBF.Models;

public partial class Student
{
    [Key]
    public int Sid { get; set; }
    public string? Name { get; set; }
    public int? Class { get; set; }
    public decimal? Fees { get; set; }
    public string? Photo { get; set; }
    public bool Status { get; set; }
}
