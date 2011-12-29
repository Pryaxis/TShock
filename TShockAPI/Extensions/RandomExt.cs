using System;
using System.Text;

namespace TShockAPI.Extensions
{
    public static class RandomExt
    {
        public static string NextString(this Random rand, int length)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                switch (rand.Next(0, 3))
                {
                    case 0:
                        sb.Append((char)rand.Next('a', 'z' + 1));
                        break;
                    case 1:
                        sb.Append((char)rand.Next('A', 'Z' + 1));
                        break;
                    case 2:
                        sb.Append((char)rand.Next('0', '9' + 1));
                        break;
                }
            }
            return sb.ToString();
        }
    }
}
