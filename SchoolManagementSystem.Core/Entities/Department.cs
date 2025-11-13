namespace SchoolManagementSystem.Core.Entities;

/// <summary>
/// Department entity for academic departments
/// </summary>
public class Department : BaseEntity
{
    public string DepartmentCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? HeadId { get; set; }
    public string? Building { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public bool IsActive { get; set; } = true;
    public int? Budget { get; set; }

    // Navigation properties
    public Teacher? Head { get; set; }
    public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
