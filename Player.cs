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
            if (pos.y < 200)
            {
                EraseCharacter();
                pos.y++;
                MoveCharacter();
            }

        }

    }
}
