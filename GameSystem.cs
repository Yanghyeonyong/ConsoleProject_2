using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleProject_2
{
    static class GameSystem
    {
        static Player player;
        public static Dictionary<int, Monster> monsterPos;



        static public void OnStart()
        {
            player = new Player();
            monsterPos = new Dictionary<int, Monster>();
            player.onGame();
        }
        static public void NewMapMonster()
        {
            monsterPos = null;
            monsterPos = new Dictionary<int, Monster>();
        }
        static public void GenerateMonster()
        {
            Monster monster = new Monster();    
            //딕셔너리에 저장한 다음에 이후 검색
            monsterPos.Add(monster.pos.x * 200 + monster.pos.y * 60, monster);
        }
        static public void GenerateMonster(int x, int y)
        {
            Monster monster = new Monster(x, y);    
            //딕셔너리에 저장한 다음에 이후 검색
            monsterPos.Add(monster.pos.x * 200 + monster.pos.y * 60, monster);
        }
        static public void GenerateBossMonster(int x, int y)
        {
            Monster monster = new Monster(x, y);  
            monster.BossMonster(x, y);
            
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
            Console.WriteLine("맞았어 : "+ monsterPos[x * 200 + y * 60].status.hp);
            if (monsterPos[x * 200 + y * 60].status.hp <= 0)
            {
                Console.WriteLine("몬스터 사망");
                player.currentExp += monsterPos[x * 200 + y * 60].exp;
                monsterPos[x * 200 + y * 60].Die();
                monsterPos[x * 200 + y * 60] = null;
                Map.MonsterMap[x, y] = false;
            }
            else 
            {
                //monsterPos[x * 200 + y * 60].SetMonsterPos(Monster.playerImage);
                Console.SetCursorPosition(x,y);
                //Console.WriteLine("출력할게");
                Console.Write(monsterPos[x * 200 + y * 60].playerImage[0]);
                Console.SetCursorPosition(x, y+1);
                Console.Write(monsterPos[x * 200 + y * 60].playerImage[1]);
            }
        }
    }
}
