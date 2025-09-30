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

        public Player()
        {
        }

        public void MoveLeft()
        { }
        public void MoveRight()
        { }
        public void MoveUp()
        { }
        public void MoveDown()
        { }

    }
}
