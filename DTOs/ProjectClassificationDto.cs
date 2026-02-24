namespace ePermitsApp.DTOs
{
    public class ProjectClassificationDto
    {
        public int Id { get; set; }
        public string ProjectClassDesc { get; set; } = null!;
    }

    public class CreateProjectClassificationDto
    {
        public string ProjectClassDesc { get; set; } = null!;
    }

    public class UpdateProjectClassificationDto
    {
        public string ProjectClassDesc { get; set; } = null!;
    }
}
