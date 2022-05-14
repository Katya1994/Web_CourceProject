namespace BlazorApp.Data;

public class SimpleChecking: IChecking
{
    public Dictionary<string, List<string>> CheckingDictionary { get; private set; } = new Dictionary<string, List<string>>();

    public double CalculateUniquenessPercent(string currentUser, Dictionary<string, List<string>> dictionary)
    {
        CheckingDictionary = dictionary;
        
        // return LineAnalyze(dictionary[currentUser], 
        //     dictionary.Where(i => i.Key != currentUser).Select(k => k.Value).ToList());
        
        return LevenshteinAnalyze(dictionary[currentUser], 
            dictionary.Where(i => i.Key != currentUser).Select(k => k.Value).ToList());
    }
    
    public List<string> CleanText(string[] lines)
    {
        List<string> list = new List<string>();

        foreach (string line in lines)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                foreach (var ch in line)
                {
                    if (char.IsLetterOrDigit(ch))
                    {
                        list.Add(line.Trim().ToLower());
                        break;
                    }
                }
            }
        }
        return list;
    }
    
    public double LineAnalyze(List<string> myList, List<List<string>> checkList)
    {
        double result = 0;

        var commonList = GetUnionList(checkList);

        // foreach (var list in checkList)
        // {
        //     commonList = commonList.Union(list).ToList();
        // }

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

        /*foreach(var list in checkLIst)
        {
            for(int i = 0; i < list.Count; i++)
            {
                for(int j = 0; j < myList.Count; j++)
                {
                    if (myList[j] == list[i])
                    {
                        result++;
                        break;
                    }                            
                }
            }
        }*/

        return result/myList.Count * 100;
    }

    public double LevenshteinAnalyze(List<string> myList, List<List<string>> checkList)
    {
        double result = 0;
        
        var commonList = GetUnionList(checkList);
        /*for(int i = 0; i < commonList.Count; i++)
        {
            for(int j = 0; j < myList.Count; j++)
            {
                var res = ComputeLevenshteinSimilarity(myList[j], commonList[i]);
                if (res >= 0.4)
                {
                    result++;
                    break;
                }
            }
        }*/
        
        for(int i = 0; i < myList.Count; i++)
        {
            for(int j = 0; j < commonList.Count; j++)
            {
                var res = ComputeLevenshteinSimilarity(myList[i], commonList[j]);
                if (res >= 0.4)
                {
                    result++;
                    break;
                }
            }
        }
        
        return result/myList.Count * 100;
    }
    
    private List<string> GetUnionList(List<List<string>> checkList)
    {
        var commonList = new List<string>();

        foreach (var list in checkList)
        {
            commonList = commonList.Union(list).ToList();
        }

        return commonList;
    }
    
    private double ComputeLevenshteinSimilarity(string source, string target)
    {
        if ((source == null) || (target == null)) return 0.0;
        if ((source.Length == 0) || (target.Length == 0)) return 0.0;
        if (source == target) return 1.0;

        int sourceWordCount = source.Length;
        int targetWordCount = target.Length;
   
        // Step 1
        if (sourceWordCount == 0)
            return targetWordCount;
   
        if (targetWordCount == 0)
            return sourceWordCount;
   
        int[,] distance = new int[sourceWordCount + 1, targetWordCount + 1];
   
        // Step 2
        for (int i = 0; i <= sourceWordCount; distance[i, 0] = i++);
        for (int j = 0; j <= targetWordCount; distance[0, j] = j++);
   
        for (int i = 1; i <= sourceWordCount; i++)
        {
            for (int j = 1; j <= targetWordCount; j++)
            {
                // Step 3
                int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;
   
                // Step 4
                distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
            }
        }
        
        return (1.0 - ((double)distance[sourceWordCount, targetWordCount] / (double)Math.Max(source.Length, target.Length)));
    }
}