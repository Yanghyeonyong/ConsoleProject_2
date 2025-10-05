using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleProject_2
{
    internal class item
    {
    }
    enum ItemType
    {
        Weapon, Shield, Nothing
    }

    class PlayerItem
    {
        public string name;
        public int hp;
        public int mp;
        public int defence;
        public int attack;
        public int criticalRate;
        public int criticalDamage;
        public ItemType type;

        //기본 생성자
        public PlayerItem()
        {
            name = "";
            attack = 0;
            defence = 0;
            criticalDamage = 0;
            criticalRate = 0;
            hp = 0;
            mp = 0;
            type = ItemType.Nothing;
        }
    }

    class Potion
    {
        //이름
        public string name;
        //hp 회복량
        public int healingHp;
        //mp 회복량
        public int healingMp;
        //공격력 증가
        public int plustDamage;
        //쿨타임 감소
        public int reduceCoolDown;
        //크리티컬확률 증가
        public int plusCriticalRate;
        //크리티컬 데미지 증가
        public int plusCriticalDamage;
        //포션 지속시간
        public int duration;

        public Potion()
        {
            name = "";
            duration = 0;
            plustDamage = 0;
            healingMp = 0;
            healingHp = 0;
            plusCriticalDamage = 0;
            plusCriticalRate = 0;
            reduceCoolDown = 0;
        }
    }
}
