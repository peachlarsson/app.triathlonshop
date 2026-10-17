namespace Peach.TriathlonShop.FunctionApp.Extensions;

public class ArticleExtensions
{
    public static string GetRandomArticle()
    {
        var articleNames = new List<string>
        {
            "Swim glasses",
            "Swim cap",
            "Swim suit",
            "Running shoes",
            "Running shorts",
            "Running shirt",
            "Bicycle",
            "Bicycle helmet",
            "Bicycle gloves",
            "Bicycle shoes",
            "Bicycle shorts",
            "Bicycle jersey",
            "Triathlon watch",
            "Energy gel",
            "Energy bar",
            "Electrolyte drink",
            "Water bottle"
        };

        int index = Random.Shared.Next(articleNames.Count);

        return $"{articleNames[index]} {GetRandomAlphabetCharacter()}";
    }

    private static char GetRandomAlphabetCharacter()
    {
        return (char)('A' + Random.Shared.Next(26));
    }
}