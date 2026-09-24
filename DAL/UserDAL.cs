using System;
using System.Data;
using System.Data.SqlClient;
using ITAssetManagement.Models;

namespace ITAssetManagement.DAL
{
    public class UserDAL
    {
        // 1. Validate login credentials
        public User GetUserByCredentials(string email, string password)
        {
            string query = @"SELECT u.UserId, u.FullName, u.Email, u.Department, u.RoleId, r.RoleName, u.IsActive 
                             FROM Users u 
                             INNER JOIN Roles r ON u.RoleId = r.RoleId 
                             WHERE u.Email = @Email AND u.PasswordHash = @Password AND u.IsActive = 1";

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        return new User
                        {
                            UserId = Convert.ToInt32(reader["UserId"]),
                            FullName = reader["FullName"].ToString(),
                            Email = reader["Email"].ToString(),
                            Department = reader["Department"]?.ToString(),
                            RoleId = Convert.ToInt32(reader["RoleId"]),
                            RoleName = reader["RoleName"].ToString(),
                            IsActive = Convert.ToBoolean(reader["IsActive"])
                        };
                    }
                }
            }
            return null;
        }

        // 2. Fetch all users for Manage Users grid
        public DataTable GetAllUsers()
        {
            DataTable dt = new DataTable();
            string query = @"SELECT u.UserId, u.Email, u.FullName, u.RoleId, r.RoleName, u.IsActive 
                             FROM Users u
                             INNER JOIN Roles r ON u.RoleId = r.RoleId
                             ORDER BY u.UserId DESC";

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        // 3. Get user details by ID
        public User GetUserById(int userId)
        {
            string query = @"SELECT u.*, r.RoleName FROM Users u 
                             INNER JOIN Roles r ON u.RoleId = r.RoleId 
                             WHERE u.UserId = @UserId";

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        return new User
                        {
                            UserId = Convert.ToInt32(reader["UserId"]),
                            Email = reader["Email"].ToString(),
                            FullName = reader["FullName"].ToString(),
                            RoleId = Convert.ToInt32(reader["RoleId"]),
                            RoleName = reader["RoleName"].ToString(),
                            IsActive = Convert.ToBoolean(reader["IsActive"])
                        };
                    }
                }
            }
            return null;
        }

        // 4. Create new user account
        public bool InsertUser(User user, string password)
        {
            string query = @"INSERT INTO Users (Email, FullName, PasswordHash, RoleId, Department, IsActive, CreatedAt) 
                    VALUES (@Email, @FullName, @Password, @RoleId, @Department, 1, GETDATE())";

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@FullName", user.FullName);
                    cmd.Parameters.AddWithValue("@Password", password); // Ensure hashed if using password security
                    cmd.Parameters.AddWithValue("@RoleId", user.RoleId);
                    cmd.Parameters.AddWithValue("@Department", string.IsNullOrWhiteSpace(user.Department) ? "General" : user.Department);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // 5. Update full name and role
        public bool UpdateUser(int userId, string fullName, int roleId)
        {
            string query = @"UPDATE Users SET FullName = @FullName, RoleId = @RoleId WHERE UserId = @UserId";

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FullName", fullName);
                    cmd.Parameters.AddWithValue("@RoleId", roleId);
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // 6. Reset password
        public bool UpdateUserPassword(int userId, string newPassword)
        {
            string query = @"UPDATE Users SET PasswordHash = @Password WHERE UserId = @UserId";

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Password", newPassword);
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool ResetPasswordByEmail(string email, string newPassword)
        {
            string query = "UPDATE Users SET PasswordHash = @Password WHERE Email = @Email AND IsActive = 1";

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", newPassword);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // 7. Change activation state (Deactivate / Reactivate)
        public bool SetUserActiveState(int userId, bool isActive)
        {
            string query = @"UPDATE Users SET IsActive = @IsActive WHERE UserId = @UserId";

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@IsActive", isActive);
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdatePasswordByEmail(string email, string newPassword)
        {
            string query = "UPDATE Users SET PasswordHash = @Password WHERE Email = @Email AND IsActive = 1";

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", newPassword);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }


    }
}