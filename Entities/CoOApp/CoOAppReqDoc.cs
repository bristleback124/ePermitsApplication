using ePermitsApp.Entities.Common;

namespace ePermitsApp.Entities.CoOApp;

public class CoOAppReqDoc : BaseEntity
{
    public int Id { get; set; }
    public int CoOAppId { get; set; }
    public string ReqDocBldgPermitSPlans { get; set; } = string.Empty;
    public string ReqDocAsBuiltPlans { get; set; } = string.Empty;
    public string ReqDocConsLogbook { get; set; } = string.Empty;
    public string ReqDocConsPhotos { get; set; } = string.Empty;
    public string ReqDocBrgyClearance { get; set; } = string.Empty;
    public string ReqDocFSIC { get; set; } = string.Empty;
    public string? ReqDocOthers { get; set; }

    public CoOApp CoOApp { get; set; } = null!;
}