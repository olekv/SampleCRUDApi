using System;
using System.ComponentModel.DataAnnotations;

namespace SampleCRUDApi.Data
{
    public class CompanyEntity : BaseEntity
    {
        public int TenantId { get; set; }
        
        [StringLength(255)] public string Industry { get; set; }
        [Required, StringLength(255)] public string Name { get; set; }
        [StringLength(255)] public string CompanyIconUri { get; set; }
        [StringLength(255)] public string Website { get; set; }
        public string CompanyInfo { get; set; }
        public string Notes { get; set; }
        
        public float Progress { get; set; }
        public int OriginalEstimate { get; set; }
        public int RemainingEstimate { get; set; }
        
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        
        public DateTime? DeletedAtUtc { get; set; }
    }
}