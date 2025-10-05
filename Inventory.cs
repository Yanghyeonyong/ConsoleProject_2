using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleProject_2
{
    internal class Inventory
    {
        //돈 가방
        public int money;
        //장비 가방
        public PlayerItem[] inventoryItem;
        //포션 가방
        public Potion[] inventoryPotion;


        public Inventory()
        {
            money = 100;
            inventoryItem = new PlayerItem[5];
            for (int i = 0; i < inventoryItem.Length; i++)
            {
                inventoryItem[i] = new PlayerItem();
            }
            inventoryPotion = new Potion[3];
            for (int i = 0; i < inventoryPotion.Length; i++)
            {
                inventoryPotion[i] = new Potion();
            }
        }
    }
}
