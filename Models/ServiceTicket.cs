using System;

namespace ITAssetManagement.Models
{
    public class ServiceTicket
    {
        public int TicketId { get; set; }
        public int TicketID { get => TicketId; set => TicketId = value; }

        public string TicketNumber { get; set; }
        public string Subject { get; set; }
        public string Title { get => Subject; set => Subject = value; }

        public string Description { get; set; }
        public int? AssetId { get; set; }
        public int ReportedByUserId { get; set; }
        public int? AssignedTechnicianId { get; set; }
        public int PolicyId { get; set; }
        public string Status { get; set; } // Open, In Progress, On Hold, Resolved, Closed
        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }

        // Display properties for GridViews & UI Joins
        public string AssetTag { get; set; }
        public string AssetName { get; set; }
        public string ReportedByName { get; set; }
        public string AssignedTechnicianName { get; set; }
        public string AssignedTo
        {
            get => string.IsNullOrEmpty(AssignedTechnicianName) ? "Unassigned" : AssignedTechnicianName;
            set => AssignedTechnicianName = value;
        }

        public string PriorityLevel { get; set; }
        public string Priority { get => PriorityLevel; set => PriorityLevel = value; }
    }
}