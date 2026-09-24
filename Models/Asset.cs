using System;

namespace ITAssetManagement.Models
{
    public class Asset
    {
        public int AssetID { get; set; }
        public string AssetTag { get; set; }
        public string AssetName { get; set; }

        // Property alias for UI compatibility
        public string DeviceName
        {
            get => AssetName;
            set => AssetName = value;
        }

        public int CategoryId { get; set; }
        public string Category { get; set; }
        public string SerialNumber { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime? WarrantyExpiryDate { get; set; }

        // Property alias for UI compatibility
        public DateTime? WarrantyExpiry
        {
            get => WarrantyExpiryDate;
            set => WarrantyExpiryDate = value;
        }

        public string Status { get; set; } // Available, Assigned, Under Repair, Retired
        public int? AssignedToUserId { get; set; }
        public string AssignedToName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}