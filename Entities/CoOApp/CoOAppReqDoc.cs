using ePermitsApp.Entities.Common;

namespace ePermitsApp.Entities.CoOApp;

public class CoOAppReqDoc : BaseEntity
{
    public int Id { get; set; }
    public int CoOAppId { get; set; }
    public string ReqDocBldgPermitSPlans { get; set; } = null!;
    public string ReqDocAsBuiltPlans { get; set; } = null!;
    public string ReqDocConsLogbook { get; set; } = null!;
    public string ReqDocConsPhotos { get; set; } = null!;
    public string ReqDocBrgyClearance { get; set; } = null!;
    public string ReqDocFSIC { get; set; } = null!;
    public string? ReqDocOthers { get; set; }

    public CoOApp CoOApp { get; set; } = null!;
}