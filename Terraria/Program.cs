namespace Terraria
{
    using System;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            using (Terraria.Main main = new Terraria.Main())
            {
                    main.Run();
            }
        }
    }
}

