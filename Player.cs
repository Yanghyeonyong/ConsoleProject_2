using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleProject_2
{
    enum Direction
    { left, right, up, down }
    struct MyPos
    {
        public int x;
        public int y;
    }
    internal class Player
    {
        string name;
        Direction dir;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public MyPos pos;

        public Player()
        {
            Name = "Nothing";
            pos.x = 0;
            pos.y = 0;
            //MoveCharacter();
            SetCharacterPos("@");
        }

        public void SetCharacterPos(string s)
        {
            Map.SetPlayerMap(pos.x, pos.y);
            Console.SetCursorPosition(pos.x, pos.y);
            Console.Write(s);
        }
        public void Move(Direction dir)
        {
            SetCharacterPos(" ");
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
            SetCharacterPos("@");
        }

        public void MoveLeft()
        {
            if (pos.x > 0)
            {
                Move(Direction.left);
            }

        }
        public void MoveRight()
        {
            if (pos.x < 200)
            {
                Move(Direction.right);
            }

        }
        public void MoveUp()
        {
            if (pos.y > 0)
            {
                Move(Direction.up);
            }

        }
        public void MoveDown()
        {
            if (pos.y < 60)
            {
                Move(Direction.down);
            }

        }

        public void Attack(int forwardRangeX, int rearRangeX, int topRangeY, int bottomRangeY)
        {
            //Map.SetAttackMap(pos,forwardRangeX, rearRangeX, topRangeY, bottomRangeY, 1);
            SetAttackMap(pos, forwardRangeX, rearRangeX, topRangeY, bottomRangeY, 1);
        }
        public static void SetAttack(int x, int y, List<int> attackRangeX, List<int> attackRangeY)
        {
            Map.AttackMap[x, y] = true;
            attackRangeX.Add(x);
            attackRangeY.Add(y);
            Console.SetCursorPosition(x, y);
            Console.Write("c");
            if (Map.AttackMap[x, y] && Map.MonsterMap[x, y])
            {
                Console.WriteLine("몬스터 공겨어어어억");
                Map.MonsterMap[x, y] = false;
            }
        }

        //이거 호출하면 해당 좌표가 true로 바뀌고 지속시간 끝나면 다시 false로 바뀐다
        public static void SetAttackMap(MyPos pos, int forwardRangeX, int rearRangeX, int topRangeY, int bottomRangeY, int duration)
        {
            BackgroundWorker attack = new BackgroundWorker();
            attack.DoWork += (sender, e) =>
            {
                int attackX;
                int attackY;
                List<int> attackRangeX = new List<int>();
                List<int> attackRangeY = new List<int>();
                for (int i = 1; i <= forwardRangeX; i++)
                {
                    attackX = pos.x + i;
                    attackY = pos.y;
                    if (attackX < 200)
                    {
                        SetAttack(attackX, attackY, attackRangeX, attackRangeY);
                    }
                    else
                    {
                        break;
                    }
                }
                for (int i = 1; i <= rearRangeX; i++)
                {
                    attackX = pos.x - i;
                    attackY = pos.y;
                    if (attackX >= 0)
                    {
                        SetAttack(attackX, attackY, attackRangeX, attackRangeY);
                    }
                    else
                    {
                        break;
                    }
                }
                for (int i = 1; i <= topRangeY; i++)
                {
                    attackX = pos.x;
                    attackY = pos.y - i;
                    if (attackY >= 0)
                    {
                        SetAttack(attackX, attackY, attackRangeX, attackRangeY);
                    }
                    else
                    {
                        break;
                    }
                }
                for (int i = 1; i <= bottomRangeY; i++)
                {
                    attackX = pos.x;
                    attackY = pos.y + i;
                    if (attackY <= 60)
                    {
                        SetAttack(attackX, attackY, attackRangeX, attackRangeY);
                    }
                    else
                    {
                        break;
                    }
                }

                Thread.Sleep(duration * 1000);

                for (int i = 0; i < attackRangeX.Count; i++)
                {
                    Console.SetCursorPosition(attackRangeX[i], attackRangeY[i]);
                    Console.Write(" ");
                    Map.AttackMap[attackRangeX[i], attackRangeY[i]] = false;
                }
                attackRangeX = null;
                attackRangeY = null;
            };
            attack.RunWorkerCompleted += (sender, e) =>
            {
                attack = null;
            };
            attack.RunWorkerAsync();
        }


    }
}
