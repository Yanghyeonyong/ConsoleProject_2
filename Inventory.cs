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

        //장비 가방 리스트로 전환
        public List<PlayerItem> inventoryItem;
        ////장비 가방
        //public PlayerItem[] inventoryItem;
        //포션 가방
        public List<Potion> inventoryPotion;
        ////포션 가방
        //public Potion[] inventoryPotion;


        public Inventory()
        {
            money = 1000;
            inventoryItem = new List<PlayerItem>();
            inventoryPotion = new List<Potion>();
            //for (int i = 0; i < inventoryItem.Length; i++)
            //{
            //    inventoryItem[i] = new PlayerItem();
            //}
            //inventoryItem = new PlayerItem[5];
            //for (int i = 0; i < inventoryItem.Length; i++)
            //{
            //    inventoryItem[i] = new PlayerItem();
            //}
            //inventoryPotion = new Potion[3];
            //for (int i = 0; i < inventoryPotion.Length; i++)
            //{
            //    inventoryPotion[i] = new Potion();
            //}
        }
    }
}
