using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleProject_2
{
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

        public Monster()
        {
            Name = "Nothing";
            pos.x = 20;
            pos.y = 20;
            MoveCharacter();
        }

        public void MoveCharacter()
        {
            Map.SetMonsterMap(pos.x, pos.y);
            Console.SetCursorPosition(pos.x, pos.y);
            Console.Write("●");
        }

        public void EraseCharacter()
        {
            Map.SetMonsterMap(pos.x, pos.y);
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
            if (pos.y < 200)
            {
                EraseCharacter();
                pos.y++;
                MoveCharacter();
            }

        }
    }
}
