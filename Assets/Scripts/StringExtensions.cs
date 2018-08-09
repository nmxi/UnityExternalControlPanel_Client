using System;
using System.Text;

public static class StringExtensions {
    public static string UnicodeToStrings(this string unicodeStrings) {

        if (unicodeStrings[0] != 'u')
            return unicodeStrings;

        string s = "";
        for (int i = 0; i < unicodeStrings.Length; i++) {
            if (unicodeStrings[i] == 'u') {
                s += ConvertCharCode(unicodeStrings[i + 1].ToString() + 
                                    unicodeStrings[i + 2].ToString() + 
                                    unicodeStrings[i + 3].ToString() + 
                                    unicodeStrings[i + 4].ToString());
            }
        }

        return s;
    }

    private static char ConvertCharCode(string charCode) {
        int charCode16 = Convert.ToInt32(charCode, 16);
        char c = Convert.ToChar(charCode16);
        return c.ToString()[0];
    }
}
