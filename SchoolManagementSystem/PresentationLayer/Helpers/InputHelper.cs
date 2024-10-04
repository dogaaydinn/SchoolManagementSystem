namespace SchoolManagementSystem.PresentationLayer.Helpers;

public static class InputHelper
{
    public static string GetValidatedStringInput(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine() ?? string.Empty;
    }

    public static int GetValidatedIntInput(string prompt)
    {
        int value;
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out value))
                return value;

            Console.WriteLine("Invalid input. Please enter a valid integer.");
        }
    }

    public static double GetValidatedDoubleInput(string prompt, double min, double max)
    {
        double value;
        while (true)
        {
            Console.Write(prompt);
            if (double.TryParse(Console.ReadLine(), out value) && value >= min && value <= max)
                return value;

            Console.WriteLine($"Invalid input. Please enter a valid number between {min} and {max}.");
        }
    }

    public static DateTime GetValidatedDateInput(string prompt)
    {
        DateTime date;
        while (true)
        {
            Console.Write(prompt);
            if (DateTime.TryParse(Console.ReadLine(), out date))
                return date;

            Console.WriteLine("Invalid date format. Please use yyyy-MM-dd.");
        }
    }
    
    public static string GetValidatedYesNoInput()
    {
        while (true)
        {
            var input = Console.ReadLine()?.Trim().ToLower();
            if (input == "yes" || input == "no")
            {
                return input;
            }
            Console.WriteLine("Invalid input. Please enter 'yes' or 'no'.");
        }
    }
}