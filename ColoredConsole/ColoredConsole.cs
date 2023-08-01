using System;

namespace ColoredConsole
{
    ///<summary>Wirite to the default console with color</summary>
    public sealed class ColoredConsole
    {
        ///<summary>Print to console with color.</summary>
        static public void Write(String Text, ConsoleColor ForegroundColor = ConsoleColor.Gray, ConsoleColor BackgroundColor = ConsoleColor.Black)
        {
            Console.ForegroundColor = ForegroundColor;

            Console.BackgroundColor = BackgroundColor;

            Console.Write(Text);
        }
    }
}