using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ITAssetManagement.Models;

namespace ITAssetManagement.DAL
{
    public class AssetDAL
    {
        // 1. Get assets assigned to specific user
        public List<Asset> GetAssetsByUser(int userId)
        {
            List<Asset> list = new List<Asset>();
            string query = @"SELECT a.*, c.CategoryName 
                             FROM Assets a
                             LEFT JOIN AssetCategories c ON a.CategoryId = c.CategoryId
                             WHERE a.AssignedToUserId = @UserId AND a.Status != 'Retired'";

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(new Asset
                        {
                            AssetID = Convert.ToInt32(reader["AssetId"]),
                            AssetTag = reader["AssetTag"].ToString(),
                            AssetName = reader["AssetName"].ToString(),
                            Category = reader["CategoryName"] != DBNull.Value ? reader["CategoryName"].ToString() : "General",
                            SerialNumber = reader["SerialNumber"]?.ToString(),
                            WarrantyExpiryDate = reader["WarrantyExpiryDate"] != DBNull.Value ? Convert.ToDateTime(reader["WarrantyExpiryDate"]) : (DateTime?)null,
                            Status = reader["Status"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        // 2. Fetch filtered asset data table for Admin GridView
        public DataTable GetFilteredAssets(string searchQuery, string category, string state)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT a.AssetId AS AssetID, a.AssetTag, a.AssetName AS DeviceName, 
                                    c.CategoryName AS Category, a.Status, u.FullName AS AssignedToName
                             FROM Assets a
                             LEFT JOIN AssetCategories c ON a.CategoryId = c.CategoryId
                             LEFT JOIN Users u ON a.AssignedToUserId = u.UserId
                             WHERE (a.AssetTag LIKE @Search OR a.AssetName LIKE @Search)
                               AND (@Category IS NULL OR c.CategoryName = @Category)
                               AND (@State IS NULL OR a.Status = @State)";

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Search", "%" + searchQuery + "%");
                    cmd.Parameters.AddWithValue("@Category", (object)category ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@State", (object)state ?? DBNull.Value);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        // 3. Get single asset record
        public Asset GetAssetById(int assetId)
        {
            string query = @"SELECT a.*, c.CategoryName 
                             FROM Assets a
                             LEFT JOIN AssetCategories c ON a.CategoryId = c.CategoryId
                             WHERE a.AssetId = @AssetId";

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AssetId", assetId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        return new Asset
                        {
                            AssetID = Convert.ToInt32(reader["AssetId"]),
                            AssetTag = reader["AssetTag"].ToString(),
                            AssetName = reader["AssetName"].ToString(),
                            Category = reader["CategoryName"]?.ToString(),
                            SerialNumber = reader["SerialNumber"]?.ToString(),
                            WarrantyExpiryDate = reader["WarrantyExpiryDate"] != DBNull.Value ? Convert.ToDateTime(reader["WarrantyExpiryDate"]) : (DateTime?)null,
                            Status = reader["Status"].ToString(),
                            AssignedToUserId = reader["AssignedToUserId"] != DBNull.Value ? Convert.ToInt32(reader["AssignedToUserId"]) : (int?)null
                        };
                    }
                }
            }
            return null;
        }

        public bool InsertAsset(Asset asset)
        {
            string query = @"INSERT INTO Assets (AssetTag, SerialNumber, AssetName, CategoryId, WarrantyExpiryDate, PurchaseDate, Status, CreatedAt)
                    VALUES (@AssetTag, @SerialNumber, @AssetName, 1, @WarrantyExpiryDate, @PurchaseDate, 'Available', GETDATE())";

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AssetTag", asset.AssetTag);
                    cmd.Parameters.AddWithValue("@SerialNumber", string.IsNullOrEmpty(asset.SerialNumber) ? asset.AssetTag : asset.SerialNumber);
                    cmd.Parameters.AddWithValue("@AssetName", asset.DeviceName);
                    cmd.Parameters.AddWithValue("@WarrantyExpiryDate", (object)asset.WarrantyExpiryDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PurchaseDate", asset.PurchaseDate != default(DateTime) ? asset.PurchaseDate : DateTime.Now);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // 5. Update existing asset details
        public bool UpdateAsset(Asset asset)
        {
            string query = @"UPDATE Assets 
                             SET AssetTag = @AssetTag, AssetName = @AssetName, WarrantyExpiryDate = @WarrantyExpiryDate 
                             WHERE AssetId = @AssetId";

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AssetTag", asset.AssetTag);
                    cmd.Parameters.AddWithValue("@AssetName", asset.DeviceName);
                    cmd.Parameters.AddWithValue("@WarrantyExpiryDate", (object)asset.WarrantyExpiryDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AssetId", asset.AssetID);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // 6. Update asset assignment user & status
        public bool UpdateAssetAssignment(int assetId, int userId, string newStatus)
        {
            string query = @"UPDATE Assets 
                             SET AssignedToUserId = @UserId, Status = @Status 
                             WHERE AssetId = @AssetId";

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId == 0 ? (object)DBNull.Value : userId);
                    cmd.Parameters.AddWithValue("@Status", newStatus);
                    cmd.Parameters.AddWithValue("@AssetId", assetId);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // 7. Update status (e.g. Retire)
        public bool UpdateAssetStatus(int assetId, string status)
        {
            string query = @"UPDATE Assets SET Status = @Status, AssignedToUserId = NULL WHERE AssetId = @AssetId";

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@AssetId", assetId);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
        public List<Asset> GetAssetsByUserId(int userId)
        {
            List<Asset> list = new List<Asset>();
            string query = "SELECT AssetId, AssetName FROM Assets WHERE AssignedToUserId = @UserId";

            using (SqlConnection conn = DbConnection.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Asset
                            {
                                AssetID = Convert.ToInt32(reader["AssetId"]),
                                DeviceName = reader["AssetName"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }
    }
}