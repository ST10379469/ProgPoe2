using ProgPoe2.Models;

public class Claim
{
    public int Id { get; set; }
    public int UserId { get; set; } // Link to User
    public string LecturerName { get; set; }
    public decimal HoursWorked { get; set; }
    public decimal HourlyRate { get; set; }
    public string Notes { get; set; }
    public string Status { get; set; } = "Pending";
    public string SupportingDocumentName { get; set; }
    public DateTime SubmissionDate { get; set; } = DateTime.Now;

    // Navigation property
    public User User { get; set; }
}