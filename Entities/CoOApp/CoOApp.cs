using System.ComponentModel.DataAnnotations;
using ePermits.Models;
using ePermitsApp.Entities.Common;

namespace ePermitsApp.Entities.CoOApp;

public class CoOApp : BaseEntity
{
    public int Id { get; set; }
    public string BldgPermitNo { get; set; } = null!;
    public string ProjectTitle { get; set; } = null!;
    public string? ProjLocBlock { get; set; }
    public string ProjLocLot { get; set; } = null!;
    public string ProjLocStreet { get; set; } = null!;

    public int ProvinceId { get; set; }
    public int LGUId { get; set; }
    public int BarangayId { get; set; }

    public int OccupancyNatureId { get; set; }
    public decimal FloorArea { get; set; }
    public int NoOfStoreys { get; set; }
    public DateTime CompletionDate { get; set; }

    public int ApplicantTypeId { get; set; }
    public string FullName { get; set; } = null!;
    [MaxLength(20)]
    public string ContactNo { get; set; } = null!;
    [MaxLength(50)]
    public string Email { get; set; } = null!;
    [MaxLength(20)]
    public string TIN { get; set; } = null!;
    public string MailAddress { get; set; } = null!;
    public string DigitalSignature { get; set; } = null!;
    public DateTime DateOfSignature { get; set; }

    public int ApplicationId { get; set; }

    // Navigation properties
    public Application? Application { get; set; }
    public Province? Province { get; set; }
    public LGU? Lgu { get; set; }
    public Barangay? Barangay { get; set; }
    public OccupancyNature? OccupancyNature { get; set; }
    public ApplicantType? ApplicantType { get; set; }

    public CoOAppProf? CoOAppProf { get; set; }
    public CoOAppReqDoc? CoOAppReqDoc { get; set; }
}