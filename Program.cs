using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleProject_2
{
    #region 2025.10.01 구현 목록
    //몬스터와 플레이어 충돌 구현
    #endregion
    internal class Program
    {
        static void Main(string[] args)
        {

            GameSetting.WindowSetting(200, 60);

            ConsoleKeyInfo key;
            Player player=new Player();
            Monster monster=new Monster();

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
                    case ConsoleKey.A:
                        player.Attack(1, 1, 1, 1);
                        break;             
                }
                
            }

        }
    }
}
