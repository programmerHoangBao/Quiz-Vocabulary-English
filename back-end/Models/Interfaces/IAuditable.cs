namespace back_end.Models.Interfaces
{
    public interface IAuditable
    {
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsDeleted { get; set; } 
    }
}
