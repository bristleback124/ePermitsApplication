namespace ePermitsApp.DTOs
{
    public class ApplicantTypeDto
    {
        public int Id { get; set; }
        public string ApplicantTypeDesc { get; set; } = null!;
    }

    public class CreateApplicantTypeDto
    {
        public string ApplicantTypeDesc { get; set; } = null!;
    }

    public class UpdateApplicantTypeDto
    {
        public string ApplicantTypeDesc { get; set; } = null!;
    }
}
