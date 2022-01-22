using HtmlAgilityPack;
using System.Text;

namespace HabrProxy.Services
{
    public class HtmlContentModifier: IContentModifier
    {
        public IWebHostEnvironment WebHostEnvironment { get; }
        private const char Modifier = '\u2122';
        private const int WordLength = 6;

        public HtmlContentModifier(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
        }

        public string Modify(string source)
        {
            if (string.IsNullOrEmpty(source)) return string.Empty;
            if (source.Length < WordLength) return source;

            var alphabetStr = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
            var alphabet = alphabetStr.ToArray().Union(alphabetStr.ToLower().ToArray()).ToArray();
            char[] surroundWordChars = new char[] {' ', '\t', '<', '>', '\n' };
            var html = new HtmlDocument();
            html.LoadHtml(source);

            var wordBuilder = new StringBuilder();
            var htmlBuilder = new StringBuilder();
            htmlBuilder.Append(html.DocumentNode.InnerHtml);
            htmlBuilder.Insert(0, " ");
            for (int i = 1; i < htmlBuilder.Length; i++) 
            {
                char currentChar = htmlBuilder[i];
                if (alphabet.Contains(currentChar)) wordBuilder.Append(currentChar);
                else
                {
                    wordBuilder.Clear();
                    continue;
                }

                if (wordBuilder.Length < WordLength) continue;

                string result = string.Empty;
                char prevWordsChar = htmlBuilder[i - WordLength];
                if (i < htmlBuilder.Length - 1)
                {
                    char nextWordsChar = htmlBuilder[i + 1];
                    if (surroundWordChars.Contains(nextWordsChar) && surroundWordChars.Contains(prevWordsChar))
                        result = wordBuilder.ToString();
                }
                else if (surroundWordChars.Contains(prevWordsChar)) result = wordBuilder.ToString();
                if (!string.IsNullOrEmpty(result))
                    htmlBuilder.Insert(i+1, Modifier);

                wordBuilder.Clear();
            }
            htmlBuilder.Remove(0, 1);
            return htmlBuilder.ToString();
        } 
    }
}
