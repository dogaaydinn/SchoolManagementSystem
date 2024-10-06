namespace SchoolManagementSystem.PresentationLayer.Helpers;

public static class InputHelper
{
    public static string GetValidatedStringInput(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input))
                return input;

            Console.WriteLine("Invalid input. Please enter a non-empty string.");
        }
    }

    public static int GetValidatedIntInput(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();
            if (int.TryParse(input, out var value))
                return value;

            Console.WriteLine("Invalid input. Please enter a valid integer.");
        }
    }

    public static double GetValidatedDoubleInput(string prompt, double min, double max)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();
            if (double.TryParse(input, out var value) && value >= min && value <= max)
                return value;

            Console.WriteLine($"Invalid input. Please enter a valid number between {min} and {max}.");
        }
    }

    public static DateTime GetValidatedDateInput(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();
            if (DateTime.TryParse(input, out var date))
                return date;

            Console.WriteLine("Invalid date format. Please use yyyy-MM-dd.");
        }
    }

    public static string GetValidatedYesNoInput()
    {
        while (true)
        {
            var input = Console.ReadLine()?.Trim().ToLower();
            if (input is "yes" or "no")
                return input;

            Console.WriteLine("Invalid input. Please enter 'yes' or 'no'.");
        }
    }
}