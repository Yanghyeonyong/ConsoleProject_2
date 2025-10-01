using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleProject_2
{
    struct MyPos
    {
        public int x;
        public int y;
    }
    internal class Player
    {
        string name;
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
            MoveCharacter();
        }

        public void MoveCharacter()
        {
            Map.SetPlayerMap(pos.x, pos.y);
            Console.SetCursorPosition(pos.x, pos.y);
            Console.Write("@");
        }

        public void EraseCharacter()
        {
            Map.SetPlayerMap(pos.x, pos.y);
            Console.SetCursorPosition(pos.x, pos.y);
            Console.Write(" ");
        }
        public void MoveLeft()
        {
            if (pos.x > 0)
            {
                EraseCharacter();
                pos.x--;
                MoveCharacter();
            }

        }
        public void MoveRight()
        {
            if (pos.x < 200)
            {
                EraseCharacter();
                pos.x++;
                MoveCharacter();
            }

        }
        public void MoveUp()
        {
            if (pos.y > 0)
            {
                EraseCharacter();
                pos.y--;
                MoveCharacter();
            }

        }
        public void MoveDown()
        {
            if (pos.y < 60)
            {
                EraseCharacter();
                pos.y++;
                MoveCharacter();
            }

        }

        public void Attack(int forwardRangeX,int rearRangeX, int topRangeY, int bottomRangeY)
        {

            Map.SetAttackMap(pos,forwardRangeX, rearRangeX, topRangeY, bottomRangeY, 1);
            ////pos.x+1부터 forwardRange까지 공격범위이며, 이는 맥스 너비를 넘어갈 경우 넘어간 것은 무시한다
            ////while 반복문으로 구현해야겠다
            //int rangeX = 1;
            //int rangeY = 1;



            ////전방 공격 범위
            //while (true)
            //{
            //    if (rangeX + pos.x < 200 && rangeX <= forwardRangeX)
            //    {
            //        Map.SetAttackMap(rangeX + pos.x, pos.y, 1);
            //        rangeX++;
            //    }
            //    else
            //    {
            //        rangeX = 1;
            //        break;
            //    }
            //}
            ////후방 공격 범위
            //while (true)
            //{
            //    if (pos.x-rangeX >=0 && rangeX <= rearRangeX)
            //    {
            //        Map.SetAttackMap(pos.x - rangeX, pos.y, 1);
            //        rangeX++;
            //    }
            //    else
            //    {
            //        rangeX = 1;
            //        break;
            //    }
            //}
            ////상단 공격 범위
            //while (true)
            //{
            //    if (pos.y- rangeY >=0 && rangeY <= topRangeY)
            //    {
            //        Map.SetAttackMap(pos.x,pos.y-rangeY, 1);
            //        rangeY++;
            //    }
            //    else
            //    {
            //        rangeY = 1;
            //        break;
            //    }
            //}
            ////전방 공격 범위
            //while (true)
            //{
            //    if (rangeY + pos.y < 60 && rangeY <= bottomRangeY)
            //    {
            //        Map.SetAttackMap(pos.x, rangeY + pos.y, 1);
            //        rangeY++;
            //    }
            //    else
            //    {
            //        rangeY = 1;
            //        break;
            //    }
            //}
            
            //Console.SetCursorPosition(pos.x, pos.y);
        }
    }
}
