using System;
using System.Data;
using System.Collections.Generic;
using ITAssetManagement.DAL;
using ITAssetManagement.Models;

namespace ITAssetManagement.BLL
{
    public class AssetBLL
    {
        private AssetDAL assetDAL = new AssetDAL();

        // 1. Retrieve user-assigned assets
        public List<Asset> GetUserAssets(int userId)
        {
            if (userId <= 0) return new List<Asset>();
            return assetDAL.GetAssetsByUser(userId);
        }

        // 2. Retrieve filtered assets for admin table view
        public DataTable GetFilteredAssets(string searchQuery, string category, string state)
        {
            // Business Rule: Standardize empty or default dropdown values
            searchQuery = searchQuery ?? string.Empty;
            if (category == "All") category = null;
            if (state == "All") state = null;

            return assetDAL.GetFilteredAssets(searchQuery, category, state);
        }

        // 3. Get single asset details
        public List<Asset> GetAssetsByUserId(int userId)
        {
            return assetDAL.GetAssetsByUserId(userId);
        }
        public Asset GetAssetById(int assetId)
        {
            if (assetId <= 0) return null;
            return assetDAL.GetAssetById(assetId);
        }

        // 4. Register new asset with auto-formatting
        public bool AddAsset(Asset asset)
        {
            if (string.IsNullOrWhiteSpace(asset.AssetTag))
            {
                asset.AssetTag = "AST-" + DateTime.Now.ToString("yyyy") + "-" + new Random().Next(1000, 9999);
            }

            if (!asset.WarrantyExpiryDate.HasValue || asset.WarrantyExpiryDate.Value == DateTime.MinValue)
            {
                asset.WarrantyExpiryDate = DateTime.Now.AddYears(3);
            }

            return assetDAL.InsertAsset(asset);
        }

        // 5. Update asset details
        public bool UpdateAsset(Asset asset)
        {
            if (asset.AssetID <= 0 || string.IsNullOrWhiteSpace(asset.DeviceName))
                return false;

            return assetDAL.UpdateAsset(asset);
        }

        // 6. Assign or return asset
        public bool AssignAsset(int assetId, int userId)
        {
            if (assetId <= 0) return false;

            // Business Rule: If userId is 0, status reverts to "Available", otherwise "Assigned"
            string newStatus = (userId == 0) ? "Available" : "Assigned";
            return assetDAL.UpdateAssetAssignment(assetId, userId, newStatus);
        }

        // 7. Retire / decommission asset
        public bool RetireAsset(int assetId)
        {
            if (assetId <= 0) return false;
            return assetDAL.UpdateAssetStatus(assetId, "Retired");
        }
    }
}