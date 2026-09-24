using System;
using System.Collections.Generic;
using ITAssetManagement.DAL;
using ITAssetManagement.Models;

namespace ITAssetManagement.BLL
{
    public class TicketBLL
    {
        private TicketDAL ticketDAL = new TicketDAL();

        // 1. Create ticket with SLA calculation
        public bool CreateTicket(ServiceTicket ticket, int priorityLevelId)
        {
            ticket.TicketNumber = "TCK-" + DateTime.Now.ToString("yyyyMMdd-HHmmss");
            ticket.PolicyId = priorityLevelId;

            int resolutionHours = 24;
            switch (priorityLevelId)
            {
                case 1: resolutionHours = 2; break;   // Critical
                case 2: resolutionHours = 8; break;   // High
                case 3: resolutionHours = 24; break;  // Medium
                case 4: resolutionHours = 48; break;  // Low
            }

            ticket.DueDate = DateTime.Now.AddHours(resolutionHours);
            return ticketDAL.CreateTicket(ticket);
        }

        // 2. Get tickets for employee dashboard
        public List<ServiceTicket> GetEmployeeTickets(int userId)
        {
            if (userId <= 0) return new List<ServiceTicket>();
            return ticketDAL.GetTicketsByEmployee(userId);
        }

        // 3. Retrieve filtered queue for tech user
        public List<ServiceTicket> GetFilteredTickets(string status, string priority)
        {
            return ticketDAL.GetFilteredTickets(status, priority);
        }

        // 4. Self-assign ticket
        public bool AssignTicketToTechnician(int ticketId, int techUserId)
        {
            if (ticketId <= 0 || techUserId <= 0) return false;
            return ticketDAL.AssignTechnician(ticketId, techUserId);
        }

        // 5. Update ticket status
        public bool UpdateStatus(int ticketId, string newStatus, int techUserId)
        {
            if (ticketId <= 0 || string.IsNullOrEmpty(newStatus)) return false;
            return ticketDAL.UpdateTicketStatus(ticketId, newStatus, techUserId);
        }

        // 6. Save technician comment
        public bool AddComment(int ticketId, int userId, string commentText)
        {
            if (ticketId <= 0 || string.IsNullOrWhiteSpace(commentText)) return false;
            return ticketDAL.AddComment(ticketId, userId, commentText);
        }
    }
}