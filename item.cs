using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleProject_2
{
    internal class item
    {
        public string name;
        public int hp;
    }

    enum ItemType
    {
        Weapon, Shield, Nothing
    }

    class PlayerItem : item
    {
        public int defence;
        public int attack;
        public ItemType type;

        public PlayerItem(string itemName, int itemAttack, int itemDefence, int itemHp, ItemType itemType)
        {
            name = itemName;
            attack = itemAttack; 
            defence = itemDefence; 
            hp = itemHp;
            type = itemType;
        }
    }

    class Potion : item
    {
        public Potion(string potionName, int potionHealingHp)
        {
            name = potionName;

            hp = potionHealingHp;

        }
    }
}
