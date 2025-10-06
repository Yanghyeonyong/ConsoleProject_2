using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleProject_2
{
// 그려지는 순서 Map -> Monster -> attack ->item -> player
    #region 2025.10.01 구현 목록
    //몬스터와 플레이어 충돌 구현
    //플레이어의 공격 구현 : 원하는 범위만큼 공격하고 몬스터가 닿으면 몬스터를 삭제 <- 이후 hp 감소로 변경 예정
    //플레이어블 캐릭터 외형 추가
    #endregion
    #region 2025.10.02 구현 목록
    //플레이어가 맵에 벽(BaseMap)이 있을 경우 넘어가지 못하는 기능 구현
    //중력 구현
    //점프 구현
    #endregion
    internal class Program
    {
        static void Main(string[] args)
        {

           GameSetting.WindowSetting(200, 60);
            //GameSetting.ShowTitle();

            bool gameStart = false;
            ConsoleKeyInfo key;
            //엔터키 입력시 게임 시작
            while (true)
            {
                key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        gameStart = true;
                        break;
                }
                if (gameStart)
                {
                    break;
                }
            }
            
            Player player = new Player();

            
            player.onGame();

            Monster monster = new Monster();

            while (true)
            {
                key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        //player.MoveUp();
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
