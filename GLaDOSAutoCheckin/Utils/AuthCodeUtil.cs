// ReSharper disable StringIndexOfIsCultureSpecific.1
namespace GLaDOSAutoCheckIn.Utils;

public static class AuthCodeUtil
{
    public static bool TryParseFromHtml(string html, out string code)
    {
        code = string.Empty;

        var startIndex = html.IndexOf("<b>");
        if (startIndex == -1)
            return false;

        var half = html[(startIndex + 3)..];

        var endIndex = half.IndexOf("</b>");
        if (endIndex == -1)
            return false;

        code = half[..endIndex];
        return true;
    }
}
