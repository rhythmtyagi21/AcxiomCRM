using System.ComponentModel.DataAnnotations;
namespace AcxiomCRM.Backend.Models;
public class Customer
{
    public int Id { get; set; }
    [Required, StringLength(100)] public string Name { get; set; } = "";
    [Required, EmailAddress, StringLength(150)] public string Email { get; set; } = "";
    [Required, RegularExpression("^[0-9]{10}$", ErrorMessage="Phone must contain exactly 10 digits.")] public string Phone { get; set; } = "";
    [Required, StringLength(120)] public string Company { get; set; } = "";
    [StringLength(100)] public string City { get; set; } = "";
    [Required] public string Status { get; set; } = "Active";
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
}
public class Lead
{
    public int Id { get; set; }
    [Required, StringLength(100)] public string Name { get; set; } = "";
    [Required, EmailAddress] public string Email { get; set; } = "";
    [Required, RegularExpression("^[0-9]{10}$")] public string Phone { get; set; } = "";
    [Required] public string Company { get; set; } = "";
    [Required] public string Source { get; set; } = "Website";
    [Required] public string Status { get; set; } = "New";
    [Required] public string Priority { get; set; } = "Medium";
    [Range(0, 1000000000)] public decimal Value { get; set; }
    public string Assigned { get; set; } = "";
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
public class Opportunity
{
    public int Id { get; set; }
    [Required, StringLength(150)] public string Name { get; set; } = "";
    [Required] public string Customer { get; set; } = "";
    [Range(typeof(decimal), "0.01", "999999999999")] public decimal Amount { get; set; }
    [Range(0, 100)] public int Probability { get; set; }
    [Required] public string Stage { get; set; } = "Qualification";
    [Required] public DateTime CloseDate { get; set; }
    [Required] public string Status { get; set; } = "Open";
    public string Assigned { get; set; } = "";
    public decimal WeightedValue => Amount * Probability / 100m;
}
public class FollowUp
{
    public int Id { get; set; }
    [Required] public string Subject { get; set; } = "";
    [Required] public string Related { get; set; } = "";
    [Required] public DateTime Date { get; set; }
    [Required] public string Type { get; set; } = "Call";
    [Required] public string Status { get; set; } = "Planned";
    public string Assigned { get; set; } = "";
    public string Notes { get; set; } = "";
}
public class Activity
{
    public int Id { get; set; }
    [Required] public string Type { get; set; } = "Call";
    [Required] public string Subject { get; set; } = "";
    public string Related { get; set; } = "";
    [Required] public DateTime Date { get; set; }
    [Required] public string Status { get; set; } = "Planned";
    public string Assigned { get; set; } = "";
}
public class AuditLog
{
    public int Id { get; set; }
    public string User { get; set; } = "System";
    public string Action { get; set; } = "";
    public string Entity { get; set; } = "";
    public string Record { get; set; } = "";
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string Result { get; set; } = "Success";
    public string? IpAddress { get; set; }
}
