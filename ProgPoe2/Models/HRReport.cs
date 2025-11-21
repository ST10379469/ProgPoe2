namespace ProgPoe2.Models
{
    public class HRReport
    {
        public DateTime ReportDate { get; set; } = DateTime.Now; // The date the report was generated
        public List<Claim> Claims { get; set; } // List of claims included in the report
        public int TotalClaims { get; set; } // Total number of claims
        public decimal TotalAmount { get; set; } // Total amount to be paid for all approved claims
        public int ApprovedClaimsCount { get; set; } // Count of approved claims
        public int PendingClaimsCount { get; set; } // Count of pending claims
        public int RejectedClaimsCount { get; set; } // Count of rejected claims
        //Troelsen and Japikse (2021)
    }
}
/*Reference List
Troelsen, A. and Japikse, P. 2021. Pro C# 9 with .NET 5: Foundational Principles and Practices in Programming. 10th ed. Apress
*/