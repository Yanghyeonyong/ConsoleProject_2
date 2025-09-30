using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleProject_2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GameSetting.WindowSetting(200, 200);

            ConsoleKeyInfo key;
            Player player=new Player();
            for (int i = 0; i < 200; i++)
            {
                Console.Write("a");
            }
            while (true)
            {
                key = Console.ReadKey(true);
                switch(key.Key) 
                    {
                    case ConsoleKey.UpArrow:
                        player.MoveUp();
                        break;
                    case ConsoleKey.DownArrow:
                        player.MoveDown();
                        break;
                    case ConsoleKey.LeftArrow:
                        player.MoveLeft();
                        break;
                    case ConsoleKey.RightArrow:
                        player.MoveRight();
                        break;

                
                }
                
            }

        }
    }
}
