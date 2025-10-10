using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleProject_2
{
    struct MyPos
    {
        public int x;
        public int y;
    }
    // 스테이터스, hp, mp, 방어력, 공격력, 치명타 확률, 치명타 데미지
    struct Status
    {
        public int hp;
        public int defence;
        public int attack;
    }

    abstract class Computer
    {
        public string name;

        public string[] playerImage;

        //해당 좌표는 좌측 상단 좌표
        public MyPos pos;
        //해당 캐릭터의 기본 스탯
        public Status myBaseStatus;

        public int exp;

        public virtual void Die()
        {
            Console.WriteLine("패배");
        }

        public virtual void MoveLeft()
        {
            Console.WriteLine("좌측 이동");
        }
        public virtual void MoveRight()
        {
            Console.WriteLine("우측 이동");
        }
        public virtual void MoveUp()
        {
            Console.WriteLine("상단 이동");
        }
        public virtual void MoveDown()
        {
            Console.WriteLine("하단 이동");
        }

        public virtual void RandomMove()
        {
            Console.WriteLine("자동 이동");
        }
    }
}
