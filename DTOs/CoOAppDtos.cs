using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ePermitsApp.DTOs
{
    public class CoOAppDto
    {
        public int Id { get; set; }
        public string BldgPermitNo { get; set; } = string.Empty;
        public string ProjectTitle { get; set; } = string.Empty;
        public string? ProjLocBlock { get; set; }
        public string ProjLocLot { get; set; } = string.Empty;
        public string ProjLocStreet { get; set; } = string.Empty;
        public int ProvinceId { get; set; }
        public int LGUId { get; set; }
        public int BarangayId { get; set; }
        public int OccupancyNatureId { get; set; }
        public decimal FloorArea { get; set; }
        public int NoOfStoreys { get; set; }
        public DateTime CompletionDate { get; set; }
        public int ApplicantTypeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string ContactNo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string TIN { get; set; } = string.Empty;
        public string MailAddress { get; set; } = string.Empty;
        public string DigitalSignature { get; set; } = string.Empty;
        public DateTime DateOfSignature { get; set; }

        public CoOAppProfDto? CoOAppProf { get; set; }
        public CoOAppReqDocDto? CoOAppReqDoc { get; set; }
    }

    public class CoOAppProfDto
    {
        public int Id { get; set; }
        public string IoCFullName { get; set; } = string.Empty;
        public string IoCPRCNo { get; set; } = string.Empty;
        public string IoCPTRNo { get; set; } = string.Empty;
        public DateTime IOCValidity { get; set; }
        public string EoRFullName { get; set; } = string.Empty;
        public string EoRPRCorPTRNo { get; set; } = string.Empty;
        public DateTime EoRValidity { get; set; }
        public string EoRSpecialization { get; set; } = string.Empty;
    }

    public class CoOAppReqDocDto
    {
        public int Id { get; set; }
        public List<FileMetadataDto> ReqDocBldgPermitSPlans { get; set; } = new();
        public List<FileMetadataDto> ReqDocAsBuiltPlans { get; set; } = new();
        public List<FileMetadataDto> ReqDocConsLogbook { get; set; } = new();
        public List<FileMetadataDto> ReqDocConsPhotos { get; set; } = new();
        public List<FileMetadataDto> ReqDocBrgyClearance { get; set; } = new();
        public List<FileMetadataDto> ReqDocFSIC { get; set; } = new();
        public List<FileMetadataDto> ReqDocOthers { get; set; } = new();
    }

    public class CoOAppCreateDto
    {
        public string? BldgPermitNo { get; set; }
        public string? ProjectTitle { get; set; }
        public string? ProjLocBlock { get; set; }
        public string? ProjLocLot { get; set; }
        public string? ProjLocStreet { get; set; }
        public int ProvinceId { get; set; }
        public int LGUId { get; set; }
        public int BarangayId { get; set; }
        public int OccupancyNatureId { get; set; }
        public decimal FloorArea { get; set; }
        public int NoOfStoreys { get; set; }
        public DateTime? CompletionDate { get; set; }
        public int ApplicantTypeId { get; set; }
        public string? FullName { get; set; }
        public string? ContactNo { get; set; }
        public string? Email { get; set; }
        public string? TIN { get; set; }
        public string? MailAddress { get; set; }
        public string? DigitalSignature { get; set; }
        public DateTime? DateOfSignature { get; set; }

        public CoOAppProfCreateDto CoOAppProf { get; set; } = new();
        public CoOAppReqDocCreateDto CoOAppReqDoc { get; set; } = new();
    }

    public class CoOAppEditDto
    {
        public int ApplicationId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string BldgPermitNo { get; set; } = string.Empty;
        public string ProjectTitle { get; set; } = string.Empty;
        public string? ProjLocBlock { get; set; }
        public string ProjLocLot { get; set; } = string.Empty;
        public string ProjLocStreet { get; set; } = string.Empty;
        public int ProvinceId { get; set; }
        public int LGUId { get; set; }
        public int BarangayId { get; set; }
        public int OccupancyNatureId { get; set; }
        public decimal FloorArea { get; set; }
        public int NoOfStoreys { get; set; }
        public DateTime CompletionDate { get; set; }
        public int ApplicantTypeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string ContactNo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string TIN { get; set; } = string.Empty;
        public string MailAddress { get; set; } = string.Empty;
        public string DigitalSignature { get; set; } = string.Empty;
        public DateTime DateOfSignature { get; set; }
        public CoOAppProfEditDto CoOAppProf { get; set; } = new();
        public CoOAppReqDocEditDto CoOAppReqDoc { get; set; } = new();
    }

    public class CoOAppUpdateDto
    {
        public string? BldgPermitNo { get; set; }
        public string? ProjectTitle { get; set; }
        public string? ProjLocBlock { get; set; }
        public string? ProjLocLot { get; set; }
        public string? ProjLocStreet { get; set; }
        public int ProvinceId { get; set; }
        public int LGUId { get; set; }
        public int BarangayId { get; set; }
        public int OccupancyNatureId { get; set; }
        public decimal FloorArea { get; set; }
        public int NoOfStoreys { get; set; }
        public DateTime? CompletionDate { get; set; }
        public int ApplicantTypeId { get; set; }
        public string? FullName { get; set; }
        public string? ContactNo { get; set; }
        public string? Email { get; set; }
        public string? TIN { get; set; }
        public string? MailAddress { get; set; }
        public string? DigitalSignature { get; set; }
        public DateTime? DateOfSignature { get; set; }
        public CoOAppProfUpdateDto CoOAppProf { get; set; } = new();
        public CoOAppReqDocUpdateDto CoOAppReqDoc { get; set; } = new();
    }

    public class CoOAppProfCreateDto
    {
        public string? IoCFullName { get; set; }
        public string? IoCPRCNo { get; set; }
        public string? IoCPTRNo { get; set; }
        public DateTime? IOCValidity { get; set; }
        public string? EoRFullName { get; set; }
        public string? EoRPRCorPTRNo { get; set; }
        public DateTime? EoRValidity { get; set; }
        public string? EoRSpecialization { get; set; }
    }

    public class CoOAppProfEditDto : CoOAppProfCreateDto
    {
    }

    public class CoOAppProfUpdateDto : CoOAppProfCreateDto
    {
    }

    public class CoOAppReqDocCreateDto
    {
        public IFormFileCollection? ReqDocBldgPermitSPlans { get; set; }
        public IFormFileCollection? ReqDocAsBuiltPlans { get; set; }
        public IFormFileCollection? ReqDocConsLogbook { get; set; }
        public IFormFileCollection? ReqDocConsPhotos { get; set; }
        public IFormFileCollection? ReqDocBrgyClearance { get; set; }
        public IFormFileCollection? ReqDocFSIC { get; set; }
        public IFormFileCollection? ReqDocOthers { get; set; }
    }

    public class CoOAppReqDocEditDto
    {
        public List<FileMetadataDto> ReqDocBldgPermitSPlans { get; set; } = new();
        public List<FileMetadataDto> ReqDocAsBuiltPlans { get; set; } = new();
        public List<FileMetadataDto> ReqDocConsLogbook { get; set; } = new();
        public List<FileMetadataDto> ReqDocConsPhotos { get; set; } = new();
        public List<FileMetadataDto> ReqDocBrgyClearance { get; set; } = new();
        public List<FileMetadataDto> ReqDocFSIC { get; set; } = new();
        public List<FileMetadataDto> ReqDocOthers { get; set; } = new();
    }

    public class CoOAppReqDocUpdateDto
    {
        public string[] KeepReqDocBldgPermitSPlans { get; set; } = Array.Empty<string>();
        public string[] KeepReqDocAsBuiltPlans { get; set; } = Array.Empty<string>();
        public string[] KeepReqDocConsLogbook { get; set; } = Array.Empty<string>();
        public string[] KeepReqDocConsPhotos { get; set; } = Array.Empty<string>();
        public string[] KeepReqDocBrgyClearance { get; set; } = Array.Empty<string>();
        public string[] KeepReqDocFSIC { get; set; } = Array.Empty<string>();
        public string[] KeepReqDocOthers { get; set; } = Array.Empty<string>();

        public IFormFileCollection? ReqDocBldgPermitSPlans { get; set; }
        public IFormFileCollection? ReqDocAsBuiltPlans { get; set; }
        public IFormFileCollection? ReqDocConsLogbook { get; set; }
        public IFormFileCollection? ReqDocConsPhotos { get; set; }
        public IFormFileCollection? ReqDocBrgyClearance { get; set; }
        public IFormFileCollection? ReqDocFSIC { get; set; }
        public IFormFileCollection? ReqDocOthers { get; set; }
    }
}
