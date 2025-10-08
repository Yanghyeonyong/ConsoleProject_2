using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleProject_2
{
    enum EnemyType
    {normal, boss }
    internal class Monster
    {
        //플레이어랑 코드 같은 것은 나중에 부모 클래스를 만들어서 상속시킬 예정
        string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public MyPos pos;

        static string[] playerImage;
        static string[] eraserplayerImage;

        public Status status;
        EnemyType enemyType;

        public Monster()
        {
            playerImage = new string[2];
            playerImage[0] = "   @ @  ";
            playerImage[1] = "( *△* )";//<-이거 특수문자 때문에 크기 8 먹는다 [2]*8 배열인듯
            eraserplayerImage = new string[2];
            eraserplayerImage[0] = "        ";
            eraserplayerImage[1] = "        ";
            Name = "슬라임";
            status.attack = 3;
            status.hp = 10;
            status.defence = 0;
            pos.x = 150;
            pos.y = 57;
            enemyType = EnemyType.normal;
            SetMonsterPos(playerImage);
        }
        public Monster(string name, int x, int y, EnemyType type)
        {
            Name = name;
            pos.x = x;
            pos.y = y;
            enemyType=type;
            SetMonsterPos(playerImage);
        }
        
        public void BossMonster()
        {
            
        }
        public void SetMonsterPos(string[] s)
        {
            Map.SetMonsterMap(pos.x, pos.y);
            Console.SetCursorPosition(pos.x, pos.y);
            Console.Write(s[0]);
            Console.SetCursorPosition(pos.x, pos.y + 1);
            Console.Write(s[1]);
        }

        public void Move(Direction dir)
        {
            Map.SetMonsterMap(pos.x, pos.y);
                Console.SetCursorPosition(pos.x, pos.y);
                for (int i = 0; i < 8; i++)
                {
                    Console.Write(Map.adventureMap[pos.x + i, pos.y]);
                }
                Console.SetCursorPosition(pos.x, pos.y + 1);
                for (int i = 0; i < 8; i++)
                {
                    Console.Write(Map.adventureMap[pos.x + i, pos.y + 1]);
                }

            switch (dir)
            {
                case Direction.left:
                    pos.x--;
                    break;
                case Direction.right:
                    pos.x++;
                    break;
                case Direction.up:
                    pos.y--;
                    break;
                case Direction.down:
                    pos.y++;
                    break;
            }


            SetMonsterPos(playerImage);
        }
        public void MoveLeft()
        {
            //특수문자라 -2다 일반이면 -1로 바꿔야 한다
            if (!Map.BaseMap[pos.x - 2, pos.y])
            {
                Move(Direction.left);
            }
        }
        public void MoveRight()
        {
            if (!Map.BaseMap[pos.x + playerImage[1].Length + 1, pos.y])
            {
                Move(Direction.right);
            }
        }

        public void Die()
        {
            Map.SetMonsterMap(pos.x, pos.y);
            Console.SetCursorPosition(pos.x, pos.y);
            for (int i = 0; i < 8; i++)
            {
                Console.Write(Map.adventureMap[pos.x + i, pos.y]);
            }
            Console.SetCursorPosition(pos.x, pos.y + 1);
            for (int i = 0; i < 8; i++)
            {
                Console.Write(Map.adventureMap[pos.x + i, pos.y + 1]);
            }
        }
    }
}
