using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleProject_2
{
    static class GameSystem
    {
        static Player player;
        static Dictionary<int, Monster> monsterPos;



        static public void OnStart()
        {
            player = new Player();
            monsterPos = new Dictionary<int, Monster>();
            player.onGame();
        }

        static public void GenerateMonster()
        {
            Monster monster = new Monster();    
            //딕셔너리에 저장한 다음에 이후 검색
            monsterPos.Add(monster.pos.x * 200 + monster.pos.y * 60, monster);
        }

        static public void ChangePos(int currentX, int currentY, int moveX, int moveY)
        {
            Monster valueMonster = monsterPos[currentX*200+currentY*60];
            monsterPos.Remove(currentX * 200 + currentY * 60);
            monsterPos[(currentX+moveX)*200 + (currentY+moveY)*60] = valueMonster;
        }

        public static void AttackMonster(int x, int y)
        {
            monsterPos[x * 200 + y * 60].status.hp -= player.myTotalStatus.attack;
            if (monsterPos[x * 200 + y * 60].status.hp <= 0)
            {
                Console.WriteLine("몬스터 사망");
                monsterPos[x * 200 + y * 60].Die();
                monsterPos[x * 200 + y * 60] = null;
                Map.MonsterMap[x, y] = false;
            }
        }
    }
}
