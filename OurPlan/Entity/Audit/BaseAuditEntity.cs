namespace OurPlan.Entity.Audit
{
    public abstract class BaseAuditEntity
    {
        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

}
