namespace ePermitsApp.DTOs
{
    public class DepartmentDto
    {
        public int Id { get; init; }
        public string DepartmentCode { get; init; } = null!;
        public string DepartmentName { get; init; } = null!;
    }
    public class CreateDepartmentDto
    {
        public string DepartmentCode { get; init; } = null!;
        public string DepartmentName { get; init; } = null!;
    }

    public class UpdateDepartmentDto
    {
        public string DepartmentCode { get; init; } = null!;
        public string DepartmentName { get; init; } = null!;
    }
}
