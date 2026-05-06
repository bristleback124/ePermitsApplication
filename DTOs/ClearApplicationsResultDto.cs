namespace ePermitsApp.DTOs
{
    public class ClearApplicationsResultDto
    {
        public Dictionary<string, int> DeletedCounts { get; set; } = new();
        public int TotalRowsDeleted { get; set; }
        public DateTime CompletedAt { get; set; }
    }
}
