namespace BlazorApp.Data;

public class ByteCodeChecking: IChecking
{
    private CheckHelper helper;
    private Dictionary<string, string> _byteCodeCheckingDictionary;
    
    public float CalculatePlagPercent(string currentUser, Dictionary<string, List<string>> dictionary)
    {
        helper = new CheckHelper();
        CleanIlCode(dictionary);

        var maxResult = GetMaxPercent(currentUser);
        
        return maxResult;
    }
    
    private void CleanIlCode(Dictionary<string, List<string>> dictionary)
    {
        _byteCodeCheckingDictionary = new Dictionary<string, string>();

        foreach (var item in dictionary)
        {
            bool isAdd = false;
            string codeText = string.Empty;

            foreach (var line in item.Value)
            {
                if (isAdd)
                    codeText += line;

                if (line.Contains("class members declaration"))
                {
                    isAdd = true;
                }
            }
            
            _byteCodeCheckingDictionary.Add(item.Key, codeText);
        }
    }

    private float GetMaxPercent(string currentUser)
    {
        float result = 0;
        var checkList = _byteCodeCheckingDictionary.Where(i => i.Key != currentUser).Select(k => k.Value);
        var currentText = _byteCodeCheckingDictionary[currentUser];
        
        foreach (var item in checkList)
        {
            var res = helper.ComputeLevenshteinSimilarity(currentText, item);

            if (res > result)
                result = res;
        }

        return result * 100;
    }
}