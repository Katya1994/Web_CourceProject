namespace BlazorApp.Data;

public interface IChecking
{
    public double CalculateUniquenessPercent(string currentUser, Dictionary<string, List<string>> dictionary);
}