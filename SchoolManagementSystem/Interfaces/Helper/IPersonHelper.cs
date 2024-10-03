using SchoolManagementSystem.Interfaces.Actions;

namespace SchoolManagementSystem.Interfaces.Helper;

public interface IPersonHelper
{
    void CheckAndDemonstrateActions(ISchoolMemberActions? person, object? user);
}