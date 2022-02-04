using HabrProxy.Services.Extensions;
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
            var englishAlphabetStr = "abcdefghijklmnopqrstuvwxyz";
            var alphabet = alphabetStr.ToArray().Union(alphabetStr.ToLower().ToArray()).ToArray()
                .Union(englishAlphabetStr.ToArray()).Union(englishAlphabetStr.ToUpper().ToArray()).ToArray();
            List<string> modifiedWords = new();
            char[] surroundWordChars = new char[] {' ', '\t', '<', '>', '\n' };
            var html = new HtmlDocument();
            html.LoadHtml(source);

            var wordBuilder = new StringBuilder();
            var htmlBuilder = new StringBuilder();
            var textHtmlBuilder = new StringBuilder();
            htmlBuilder.Append(html.DocumentNode.InnerHtml);
            htmlBuilder.Insert(0, " ");
            htmlBuilder.Append(" ");
            textHtmlBuilder.Append(html.DocumentNode.InnerText);
            textHtmlBuilder.Insert(0, " ");
            textHtmlBuilder.Append(" ");

            for (int i = 0; i < textHtmlBuilder.Length; i++) 
            {
                var currentChar = textHtmlBuilder[i];
                if (alphabet.Contains(currentChar)) wordBuilder.Append(currentChar);
                else
                {
                    wordBuilder.Clear();
                    continue;
                }
                if (wordBuilder.Length < WordLength) continue;

                string result = string.Empty;
                char prevWordsChar = textHtmlBuilder[i - WordLength];
                if (i < textHtmlBuilder.Length - 1)
                {
                    char nextWordsChar = textHtmlBuilder[i + 1];
                    if (surroundWordChars.Contains(nextWordsChar) && surroundWordChars.Contains(prevWordsChar))
                        result = wordBuilder.ToString();
                }
                else if (surroundWordChars.Contains(prevWordsChar)) result = wordBuilder.ToString();
                if (!string.IsNullOrEmpty(result) && !modifiedWords.Contains(result))
                {
                    var idx = htmlBuilder.ToString().AllIndexesOf(result);
                    for (int j = 0; j < idx.Count; j++) 
                    {
                        htmlBuilder.Insert(idx[j]+j + WordLength, Modifier);
                    }

                    modifiedWords.Add(result);
                }
            }
            htmlBuilder.Remove(0, 1);
            htmlBuilder.Remove(htmlBuilder.Length - 1, 1);
            return htmlBuilder.ToString();
        }

        
    }
}
