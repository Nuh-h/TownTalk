namespace TownTalk.Web.Helpers;

public class Profile
{
    private static readonly char[] Separator = new[] { ' ' };

    public static string GetProfilePictureUrl(string name)
    {
        string? profileName = name.Split(Separator, StringSplitOptions.RemoveEmptyEntries)
                         .Select(w => w[0])
                         .Take(2)
                         .Aggregate(string.Empty, (a, b) => a + b)
                         ?? name[..Math.Min(3, name.Length)];

        return $"https://placehold.co/800?text={profileName}&font=Lora";
    }
}