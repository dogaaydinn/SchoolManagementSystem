using SchoolManagementSystem.Interfaces;

namespace SchoolManagementSystem.Models.Abstract;

public abstract class Person : IPersonActions
{
    protected Person(string firstName, string lastName, DateTime dateOfBirth)
    {
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
    }

    private string FirstName { get; }
    private string LastName { get; }
    private DateTime DateOfBirth { get; }

    public void PerformDailyRoutine()
    {
        Console.WriteLine($"{GetFullName()} is performing daily routine.");
    }
    
    public void Communicate()
    {
        Console.WriteLine($"{GetFullName()} is communicating.");
    }
    
    public void Rest()
    {
        Console.WriteLine($"{GetFullName()} is resting.");
    }

    protected string GetFullName()
    {
        return $"{FirstName} {LastName}";
    }

    public override string ToString()
    {
        return GetFullName();
    }

    protected DateTime GetDateOfBirth()
    {
        return DateOfBirth;
    }
}