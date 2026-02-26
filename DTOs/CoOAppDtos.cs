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
        public string ReqDocBldgPermitSPlans { get; set; } = string.Empty;
        public string ReqDocAsBuiltPlans { get; set; } = string.Empty;
        public string ReqDocConsLogbook { get; set; } = string.Empty;
        public string ReqDocConsPhotos { get; set; } = string.Empty;
        public string ReqDocBrgyClearance { get; set; } = string.Empty;
        public string ReqDocFSIC { get; set; } = string.Empty;
        public string? ReqDocOthers { get; set; }
    }

    public class CoOAppCreateDto
    {
        [Required]
        public string BldgPermitNo { get; set; } = string.Empty;
        [Required]
        public string ProjectTitle { get; set; } = string.Empty;
        public string? ProjLocBlock { get; set; }
        [Required]
        public string ProjLocLot { get; set; } = string.Empty;
        [Required]
        public string ProjLocStreet { get; set; } = string.Empty;
        [Required]
        public int ProvinceId { get; set; }
        [Required]
        public int LGUId { get; set; }
        [Required]
        public int BarangayId { get; set; }
        [Required]
        public int OccupancyNatureId { get; set; }
        [Required]
        public decimal FloorArea { get; set; }
        [Required]
        public int NoOfStoreys { get; set; }
        [Required]
        public DateTime CompletionDate { get; set; }
        [Required]
        public int ApplicantTypeId { get; set; }
        [Required]
        public string FullName { get; set; } = string.Empty;
        [Required]
        public string ContactNo { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string TIN { get; set; } = string.Empty;
        [Required]
        public string MailAddress { get; set; } = string.Empty;
        [Required]
        public string DigitalSignature { get; set; } = string.Empty;
        [Required]
        public DateTime DateOfSignature { get; set; }

        public CoOAppProfCreateDto CoOAppProf { get; set; } = null!;
        public CoOAppReqDocCreateDto CoOAppReqDoc { get; set; } = null!;
    }

    public class CoOAppProfCreateDto
    {
        [Required]
        public string IoCFullName { get; set; } = string.Empty;
        [Required]
        public string IoCPRCNo { get; set; } = string.Empty;
        [Required]
        public string IoCPTRNo { get; set; } = string.Empty;
        [Required]
        public DateTime IOCValidity { get; set; }
        [Required]
        public string EoRFullName { get; set; } = string.Empty;
        [Required]
        public string EoRPRCorPTRNo { get; set; } = string.Empty;
        [Required]
        public DateTime EoRValidity { get; set; }
        [Required]
        public string EoRSpecialization { get; set; } = string.Empty;
    }

    public class CoOAppReqDocCreateDto
    {
        [Required]
        public IFormFile ReqDocBldgPermitSPlans { get; set; } = null!;
        [Required]
        public IFormFile ReqDocAsBuiltPlans { get; set; } = null!;
        [Required]
        public IFormFile ReqDocConsLogbook { get; set; } = null!;
        [Required]
        public IFormFile ReqDocConsPhotos { get; set; } = null!;
        [Required]
        public IFormFile ReqDocBrgyClearance { get; set; } = null!;
        [Required]
        public IFormFile ReqDocFSIC { get; set; } = null!;
        public IFormFile? ReqDocOthers { get; set; }
    }
}
