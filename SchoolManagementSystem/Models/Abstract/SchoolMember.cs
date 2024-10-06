namespace SchoolManagementSystem.Models.Abstract;

public abstract class SchoolMember : Person
{
    #region Constructors

    protected SchoolMember(string firstName, string lastName, DateTime dateOfBirth, string password)
        : base(firstName, lastName, dateOfBirth, false, password)
    {
    }

    #endregion
    #region Methods

    protected int GetAge()
    {
        var today = DateTime.Today;
        var age = today.Year - GetDateOfBirth().Year;
        if (GetDateOfBirth().Date > today.AddYears(-age)) age--;
        return age;
    }

    public abstract override void DisplayDetails();

    #endregion
}