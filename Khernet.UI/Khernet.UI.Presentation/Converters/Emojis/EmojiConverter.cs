using System;

namespace Khernet.UI.Cache
{
    public class EmojiConverter
    {
        /// <summary>
        /// Converts a emoji from unicode code points
        /// </summary>
        /// <param name="codePoint">A string containing the emoji</param>
        /// <returns></returns>
        public string ConvertToString(string codePoint)
        {
            if (string.IsNullOrEmpty(codePoint) || string.IsNullOrWhiteSpace(codePoint))
                return null;

            string[] codePointList = codePoint.Split('_');

            string emoji = null;

            for (int i = 0; i < codePointList.Length; i++)
            {
                emoji += Char.ConvertFromUtf32(Convert.ToInt32(codePointList[i], 16));
            }

            return emoji;
        }

        public string ConvertUnicodeCodePoint(string encodedValue)
        {
            if (string.IsNullOrEmpty(encodedValue) || string.IsNullOrWhiteSpace(encodedValue))
                return null;

            string emojiCodes = null;

            string separator = "_";

            for (int i = 0; i < encodedValue.Length; i++)
            {
                if (Char.IsSurrogatePair(encodedValue, i))
                {
                    emojiCodes = JoinUnicodePointCode(emojiCodes, separator, Convert.ToString(Char.ConvertToUtf32(encodedValue[i], encodedValue[i + 1]), 16));
                    i++;
                }
                else
                    emojiCodes = JoinUnicodePointCode(emojiCodes, separator, Convert.ToString(Char.ConvertToUtf32(encodedValue, i), 16));
            }

            return emojiCodes;
        }

        private string JoinUnicodePointCode(string code1, string separator, string code2)
        {
            string emojiCode = null;
            if (!string.IsNullOrEmpty(code1) && !string.IsNullOrEmpty(code2))
            {
                emojiCode = string.Join(separator, code1, code2);
            }
            else
                emojiCode = !string.IsNullOrEmpty(code1) ? code1 : code2;

            return emojiCode;
        }
    }
}
