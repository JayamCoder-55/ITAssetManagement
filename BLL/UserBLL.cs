using ITAssetManagement.DAL;
using ITAssetManagement.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ITAssetManagement.BLL
{
    public class UserBLL
    {
        private UserDAL userDAL = new UserDAL();

        // 1. Authenticate user credentials
        public User ValidateLogin(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            return userDAL.GetUserByCredentials(email, password);
        }

        // 2. Get all system users
        public DataTable GetAllUsers()
        {
            return userDAL.GetAllUsers();
        }

        // 3. Get user details by ID
        public User GetUserById(int userId)
        {
            if (userId <= 0) return null;
            return userDAL.GetUserById(userId);
        }

        // 4. Create new user account
        public bool CreateUser(User user, string defaultPassword)
        {
            if (string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(defaultPassword))
                return false;

            return userDAL.InsertUser(user, defaultPassword);
        }

        // 5. Update user information and system role
        public bool UpdateUser(int userId, string fullName, int roleId)
        {
            if (userId <= 0 || string.IsNullOrWhiteSpace(fullName))
                return false;

            return userDAL.UpdateUser(userId, fullName, roleId);
        }

        // 6. Reset password and generate temporary credential
        public string ResetPassword(int userId)
        {
            if (userId <= 0) return null;

            // Business Rule: Auto-generate a 10-character temp password
            string tempPassword = "Temp!" + Guid.NewGuid().ToString().Substring(0, 6);
            bool success = userDAL.UpdateUserPassword(userId, tempPassword);

            return success ? tempPassword : null;
        }

        // 7. Deactivate user account
        public bool DeactivateUser(int userId)
        {
            if (userId <= 0) return false;
            return userDAL.SetUserActiveState(userId, false);
        }

        public bool ResetPasswordByEmail(string email, string newPassword)
        {
            return userDAL.ResetPasswordByEmail(email, newPassword);
        }

        public bool UpdatePasswordByEmail(string email, string newPassword)
        {
            return userDAL.UpdatePasswordByEmail(email, newPassword);
        }

        public bool UpdateUserStatus(int userId, bool isActive)
        {
            string query = "UPDATE Users SET IsActive = @IsActive WHERE UserId = @UserId";

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@IsActive", isActive ? 1 : 0);
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}