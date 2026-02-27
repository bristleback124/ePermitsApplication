using System.ComponentModel.DataAnnotations;
using ePermitsApp.Entities.Common;

namespace ePermitsApp.Entities.CoOApp;

public class CoOAppProf : BaseEntity
{
    public int Id { get; set; }
    public int CoOAppId { get; set; }
    public string IoCFullName { get; set; } = null!;
    
    [MaxLength(20)]
    public string IoCPRCNo { get; set; } = null!;
    
    [MaxLength(20)]
    public string IoCPTRNo { get; set; } = null!;

    public DateTime IOCValidity { get; set; }
    
    public string EoRFullName { get; set; } = null!;

    [MaxLength(20)]
    public string EoRPRCorPTRNo { get; set; } = null!;

    public DateTime EoRValidity { get; set; }

    public string EoRSpecialization { get; set; } = null!;
    
    public CoOApp CoOApp { get; set; } = null!;
}