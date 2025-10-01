using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleProject_2
{
    internal static class Map
    {
        //플레이어와 몬스터의 위치를 추적하여 충돌을 확인하기 위해 사용
        static bool[,] playerMap;
        static bool[,] monsterMap;

        public static void InitMap(int width, int height) 
        {
            playerMap= new bool[width, height];
            monsterMap= new bool[width, height];
        }

        public static void SetMonsterMap(int x, int y)
        {
            //참고로 몬스터는 여러개 있는 경우 해당 몬스터 위치로 이동하지 않도록 몬스터 클래스에 구현
            //true인 경우 false <- 이동 시작점이라 지나쳤으므로 false로 변환
            if (monsterMap[x, y])
            {
                monsterMap[x, y] = false;
            }
            //false인 경우 true <- 도착점이므로 true로 변환
            else
            {
                monsterMap[x, y] = true;
            }

            //몬스터와 플레이어가 충돌 시
            if (monsterMap[x, y] && playerMap[x, y])
            {
                //발생할 코드 작성 예정
                //몬스터가 들이 박은거니 플레이어한테 부정적 영향
                Console.Write("충돌 발생:Monster");
            }
        }
        public static void SetPlayerMap(int x, int y)
        {
            //true인 경우 false <- 이동 시작점이라 지나쳤으므로 false로 변환
            if (playerMap[x, y])
            {
                playerMap[x, y] = false;
            }
            //false인 경우 true <- 도착점이므로 true로 변환
            else
            {
                playerMap[x, y] = true;

                //몬스터와 플레이어가 충돌 시
                if (monsterMap[x, y] && playerMap[x, y])
                {
                    //발생할 코드 작성 예정
                    //플레이어가 들이 박은거니 플레이어한테 부정적 영향
                    
                    //해당 문자열 출력 확인 -> 충돌 기능 정상 작동
                    Console.Write("충돌 발생:Player");
                }
            }


        }

    }
}
