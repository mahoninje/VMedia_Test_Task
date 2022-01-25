namespace HabrProxy.Services.Extensions
{
    public static class ListHelpers
    {
        public static List<int> AllIndexesOf(this string str, string value)
        {
            var worldLen = value.Length;
            char[] surroundWordChars = new char[] { ' ', '\t', '<', '>', '\n' };
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;

                if (surroundWordChars.Contains(str[index-1]) && surroundWordChars.Contains(str[index+worldLen]))
                    indexes.Add(index);
            }
        }
    }
}
