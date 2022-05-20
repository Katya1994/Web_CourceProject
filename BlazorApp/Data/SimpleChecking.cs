using System.Diagnostics;

namespace BlazorApp.Data;

public class SimpleChecking: IChecking
{
    private CheckHelper helper;
    public Dictionary<string, List<string>> CheckingDictionary { get; private set; } = new Dictionary<string, List<string>>();

    public float CalculatePlagPercent(string currentUser, Dictionary<string, List<string>> dictionary)
    {
        try
        {
            helper = new CheckHelper();
            
            CheckingDictionary = dictionary;

            return LevenshteinAnalyze(dictionary[currentUser],
                dictionary.Where(i => i.Key != currentUser).Select(k => k.Value).ToList());
        }
        catch (Exception ex)
        {
            Debug.Print(ex.Message);
            return float.MinValue;
        }
    }

    public float LineAnalyze(List<string> myList, List<List<string>> checkList)
    {
        float result = 0;

        var commonList = helper.GetUnionList(checkList);

        for(int i = 0; i < commonList.Count; i++)
        {
            for(int j = 0; j < myList.Count; j++)
            {
                if (myList[j] == commonList[i])
                {
                    result++;
                    break;
                }                            
            }
        }

        return result/myList.Count * 100;
    }

    public float LevenshteinAnalyze(List<string> myList, List<List<string>> checkList)
    {
        float result = 0;
        
        var commonList = helper.GetUnionList(checkList);

        for(int i = 0; i < myList.Count; i++)
        {
            for(int j = 0; j < commonList.Count; j++)
            {
                var res = helper.ComputeLevenshteinSimilarity(myList[i], commonList[j]);
                if (res >= 0.7)
                {
                    result++;
                    break;
                }
            }
        }
        
        return result/myList.Count * 100;
    }
    
    
}