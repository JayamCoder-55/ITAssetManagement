using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ITAssetManagement.Models;

namespace ITAssetManagement.DAL
{
    public class TicketDAL
    {
        // 1. Create a new service ticket
        public bool CreateTicket(ServiceTicket ticket)
        {
            string query = @"INSERT INTO ServiceTickets (TicketNumber, Subject, Description, AssetId, ReportedByUserId, PolicyId, Status, DueDate, CreatedAt)
                             VALUES (@TicketNumber, @Subject, @Description, @AssetId, @ReportedByUserId, @PolicyId, 'Open', @DueDate, GETDATE())";

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TicketNumber", ticket.TicketNumber);
                    cmd.Parameters.AddWithValue("@Subject", ticket.Subject);
                    cmd.Parameters.AddWithValue("@Description", ticket.Description);
                    cmd.Parameters.AddWithValue("@AssetId", (!ticket.AssetId.HasValue || ticket.AssetId.Value <= 0) ? (object)DBNull.Value : ticket.AssetId.Value);
                    cmd.Parameters.AddWithValue("@ReportedByUserId", ticket.ReportedByUserId);
                    cmd.Parameters.AddWithValue("@PolicyId", ticket.PolicyId);
                    cmd.Parameters.AddWithValue("@DueDate", ticket.DueDate);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // 2. Fetch tickets submitted by a specific employee
        public List<ServiceTicket> GetTicketsByEmployee(int userId)
        {
            List<ServiceTicket> list = new List<ServiceTicket>();
            string query = @"SELECT t.*, a.AssetTag, a.AssetName, p.PriorityLevel 
                             FROM ServiceTickets t
                             LEFT JOIN Assets a ON t.AssetId = a.AssetId
                             INNER JOIN SLAPolicies p ON t.PolicyId = p.PolicyId
                             WHERE t.ReportedByUserId = @UserId
                             ORDER BY t.CreatedAt DESC";

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(new ServiceTicket
                        {
                            TicketId = Convert.ToInt32(reader["TicketId"]),
                            TicketNumber = reader["TicketNumber"].ToString(),
                            Subject = reader["Subject"].ToString(),
                            Description = reader["Description"].ToString(),
                            AssetTag = reader["AssetTag"]?.ToString(),
                            AssetName = reader["AssetName"]?.ToString(),
                            PriorityLevel = reader["PriorityLevel"].ToString(),
                            Status = reader["Status"].ToString(),
                            DueDate = Convert.ToDateTime(reader["DueDate"]),
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                        });
                    }
                }
            }
            return list;
        }

        // 3. Get filtered queue for technicians
        public List<ServiceTicket> GetFilteredTickets(string status, string priority)
        {
            List<ServiceTicket> list = new List<ServiceTicket>();
            string query = @"SELECT t.*, a.AssetTag, uReport.FullName AS ReportedByName, 
                                    uTech.FullName AS AssignedTechnicianName, p.PriorityLevel 
                             FROM ServiceTickets t
                             LEFT JOIN Assets a ON t.AssetId = a.AssetId
                             INNER JOIN Users uReport ON t.ReportedByUserId = uReport.UserId
                             LEFT JOIN Users uTech ON t.AssignedTechnicianId = uTech.UserId
                             INNER JOIN SLAPolicies p ON t.PolicyId = p.PolicyId
                             WHERE (@Status = 'All' OR t.Status = @Status)
                               AND (@Priority = 'All' OR p.PriorityLevel = @Priority)
                             ORDER BY t.DueDate ASC";

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Status", string.IsNullOrEmpty(status) ? "All" : status);
                    cmd.Parameters.AddWithValue("@Priority", string.IsNullOrEmpty(priority) ? "All" : priority);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(new ServiceTicket
                        {
                            TicketId = Convert.ToInt32(reader["TicketId"]),
                            TicketNumber = reader["TicketNumber"].ToString(),
                            Subject = reader["Subject"].ToString(),
                            Description = reader["Description"]?.ToString(),
                            ReportedByName = reader["ReportedByName"]?.ToString(),
                            AssignedTechnicianName = reader["AssignedTechnicianName"] != DBNull.Value ? reader["AssignedTechnicianName"].ToString() : null,
                            PriorityLevel = reader["PriorityLevel"]?.ToString(),
                            Status = reader["Status"].ToString(),
                            DueDate = Convert.ToDateTime(reader["DueDate"])
                        });
                    }
                }
            }
            return list;
        }

        // 4. Assign ticket to technician
        public bool AssignTechnician(int ticketId, int techId)
        {
            string query = @"UPDATE ServiceTickets 
                             SET AssignedTechnicianId = @TechId, 
                                 Status = CASE WHEN Status = 'Open' THEN 'In Progress' ELSE Status END 
                             WHERE TicketId = @TicketId";

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TechId", techId);
                    cmd.Parameters.AddWithValue("@TicketId", ticketId);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // 5. Update ticket status
        public bool UpdateTicketStatus(int ticketId, string status, int techId)
        {
            string query = @"UPDATE ServiceTickets 
                             SET Status = @Status, 
                                 AssignedTechnicianId = ISNULL(AssignedTechnicianId, @TechId), 
                                 ResolvedAt = CASE WHEN @Status = 'Resolved' THEN GETDATE() ELSE ResolvedAt END
                             WHERE TicketId = @TicketId";

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@TechId", techId);
                    cmd.Parameters.AddWithValue("@TicketId", ticketId);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // 6. Add technician note / comment
        public bool AddComment(int ticketId, int userId, string commentText)
        {
            string query = @"INSERT INTO TicketComments (TicketId, UserId, CommentText, CreatedAt)
                             VALUES (@TicketId, @UserId, @CommentText, GETDATE())";

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TicketId", ticketId);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@CommentText", commentText);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // 7. Get past comments / notes for a specific ticket
        // Get past comments / notes for a specific ticket (including initial problem description)
        public DataTable GetTicketComments(int ticketId)
        {
            DataTable dt = new DataTable();
            string query = @"
        SELECT 
            t.Description AS CommentText, 
            t.CreatedAt, 
            u.FullName + ' (Initial Report)' AS FullName 
        FROM ServiceTickets t
        INNER JOIN Users u ON t.ReportedByUserId = u.UserId
        WHERE t.TicketId = @TicketId

        UNION ALL

        SELECT 
            c.CommentText, 
            c.CreatedAt, 
            u.FullName 
        FROM TicketComments c
        INNER JOIN Users u ON c.UserId = u.UserId
        WHERE c.TicketId = @TicketId

        ORDER BY CreatedAt ASC";

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TicketId", ticketId);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }
    }
}