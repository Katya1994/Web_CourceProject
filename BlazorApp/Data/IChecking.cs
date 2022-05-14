namespace BlazorApp.Data;

public interface IChecking
{
    public float CalculatePlagPercent(string currentUser, Dictionary<string, List<string>> dictionary);
}