namespace BlazorApp.Data;

public interface IChecking
{
    public double CalculatePlagPercent(string currentUser, Dictionary<string, List<string>> dictionary);
}