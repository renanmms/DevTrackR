using System;

namespace DevTrackR.API.Entities
{
    public class PackageUpdate
    {
        public PackageUpdate(string status, int packageID)
        {
            Status = status;
            UpdateDate = DateTime.Now;
            PackageID = packageID;
        }
        public int PackageID { get; private set; }
        public int Id { get; private set; }
        public string Status { get; private set; }
        public DateTime UpdateDate { get; private set; }
    }
}