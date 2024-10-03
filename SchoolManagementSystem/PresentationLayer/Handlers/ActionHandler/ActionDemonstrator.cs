namespace SchoolManagementSystem.PresentationLayer.Handlers.ActionHandler;

public static class ActionDemonstrator
{
    public static void DemonstrateActions(string actionType, Action[] actions)
    {
        try
        {
            Console.WriteLine($"Demonstrating {actionType} actions:");
            foreach (var action in actions)
            {
                action();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while demonstrating {actionType} actions: {ex.Message}");
        }
    }
}