using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleProject_2
{
    enum Direction
    { left, right, up, down }

    //플레이어가 착용한 아이템
    struct Equipment
    {
        public List<PlayerItem> myItem;
        public Potion myPotion;
    }

    internal class Player : Computer
    {
        Direction dir;

        public bool onAdventure;
        public bool onVillage;
        public int currentStage;

        //전방을 바라보고 있는지 확인
        public bool onFront;

        //캐릭터 자체의 스탯 + 아이템 스탯<-향후 아이템 추가 후 메서드 추가
        public Status myTotalStatus;

        //인벤토리
        public Inventory inventory;

        //착용 장비
        public Equipment equipment;
        const int MaxEquipment = 5;

        //현재 체력, 위에는 맥스 체력임
        int currentHp;
        //경험치 및 레벨
        int level;
        int maxExp;
        int exp;
        int statPoint;


        public Player()
        {
            playerImage = new string[2];
            playerImage[0] = " ∧ ∧";
            playerImage[1] = "( '▽' )";//<-이거 특수문자 때문에 크기 8 먹는다 [2]*8 배열인듯
            name = "Nothing";
            pos.x = 100;
            pos.y = 30;
        }

        #region 조작

        public void Jump()
        {
            for (int i = 0; i < 10; i++)
            {
                MoveUp();
            }

        }

        public void UsingPotion()
        {
            if (equipment.myPotion != null)
            {
                currentHp += equipment.myPotion.hp;
                if (currentHp > myTotalStatus.hp)
                {
                    currentHp = myTotalStatus.hp;
                }
            }
            DrawMyInformation();
        }
        public void DrawCharacter()
        {
            Console.SetCursorPosition(pos.x, pos.y);
            Console.Write(playerImage[0]);
            Console.SetCursorPosition(pos.x, pos.y + 1);
            Console.Write(playerImage[1]);
        }

        void SetCharacterPos()
        {
            Map.SetPlayerMap(pos.x, pos.y);
            DrawCharacter();
        }

        void RestoreMap(char[,] map)
        {
            Console.SetCursorPosition(pos.x, pos.y);
            for (int i = 0; i <= playerImage[1].Length; i++)
            {
                Console.Write(map[pos.x + i, pos.y]);
            }
            Console.SetCursorPosition(pos.x, pos.y + 1);
            for (int i = 0; i <= playerImage[1].Length; i++)
            {
                Console.Write(map[pos.x + i, pos.y + 1]);
            }
        }

        void RestoreText(int x, int y)
        {
            for (int i = 0; i < 5; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write(Map.NextStage[i]);
            }
        }

        void Move(Direction dir)
        {
            Map.SetPlayerMap(pos.x,pos.y);
            if (onVillage)
            {
                RestoreMap(Map.villageMap);
            }
            else if (onAdventure)
            {
                RestoreMap(Map.adventureMap);

                if (currentStage == 1)
                {
                    RestoreText(Map.BaseMap.GetLength(0) - 30, 9);
                }
                if (currentStage == 2)
                {
                    RestoreText(170, 54);
                }
                DrawMyInformation();
            }

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

            SetCharacterPos();
        }


        public void MoveLeft()
        {
            if (!Map.BaseMap[pos.x - 2, pos.y] 
                && EnableMoveRangeY(Math.Max(0, pos.x - 7),pos.y))
            {
                Move(Direction.left);
                if (!Map.monsterAttackMap[pos.x, pos.y])
                {
                    
                    Move(Direction.right);
                    Move(Direction.right);
                    Console.SetCursorPosition(pos.x-2, pos.y+1);
                    Console.Write(')');
                    GameSystem.AttackPlayer(GameSystem.monsterPos[(pos.x-9) * 200 + pos.y * 60].myBaseStatus.attack);
                    if (onAdventure)
                    {
                        DrawMyInformation();
                    }
                }
            }
        }
        public void MoveRight()
        {
            if (!Map.BaseMap[pos.x + playerImage[1].Length + 1 , pos.y] 
                && EnableMoveRangeY(Math.Max(0, pos.x) + playerImage[1].Length-1, pos.y))
            {
                Move(Direction.right);
                if (!Map.monsterAttackMap[pos.x+8, pos.y])
                {
                    Move(Direction.left);
                    Move(Direction.left);
                    Console.SetCursorPosition(pos.x + 9, pos.y + 1);
                    Console.Write('(');
                    GameSystem.AttackPlayer(GameSystem.monsterPos[(pos.x +9) * 200 + pos.y * 60].myBaseStatus.attack);
                    if (onAdventure)
                    {
                        DrawMyInformation();
                    }
                }
            }
        }
        public void MoveUp()
        {
            if (!Map.BaseMap[pos.x, pos.y - playerImage.Length + 1]&&EnableMoveRangeX(pos.x, pos.y - playerImage.Length+1))
            {
                Move(Direction.up);
            }

        }
        public void MoveDown()
        {
            if (!Map.BaseMap[pos.x, pos.y + playerImage.Length]
                &&EnableMoveRangeX(pos.x, pos.y + playerImage.Length))
            {
                Move(Direction.down);
            }
        }

        public bool EnableMoveRangeX(int x, int y)
        {
            for (int i = -playerImage[1].Length; i <= playerImage[1].Length; i++)
            {
                if (Map.MonsterMap[Math.Max(0, x + i), y])
                {
                    return false;
                }
            }
            return true;
        }
        public bool EnableMoveRangeY(int x, int y)
        {
            for (int i = -playerImage.Length+1; i <= playerImage.Length-1; i++)
            {
                if (Map.MonsterMap[x, y+i])
                {
                    return false;
                }
            }
            return true;
        }

        public void Attack(int forwardRangeX, int rearRangeX, int topRangeY, int bottomRangeY, List<int> attackX, List<int> attackY)
        {
            int forward = 0;
            //전방 공격
            for (int i = 1; i <= forwardRangeX; i++)
            {
                forward = pos.x + playerImage[1].Length + i;
                if (forward >= 198)
                {
                    break;
                }
                if (AttackRange(forward, pos.y, 0, attackX, attackY))
                {
                    break;
                }
            }

            int rear = 0;
            //후방 공격
            for (int i = 1; i <= rearRangeX; i++)
            {
                rear = pos.x - i;
                if (rear <= 2)
                {
                    break;
                }
                if (AttackRange(rear, pos.y, playerImage[1].Length, attackX, attackY))
                {
                    break;
                }
            }

        }

        public bool AttackRange(int rangeX, int rangeY, int rearRange, List<int> attackX, List<int> attackY)
        {

            for (int j = 0; j < playerImage.Length; j++)
                {
                Map.playerAttackMap[rangeX, rangeY+j] = true;

                    if (Map.playerAttackMap[rangeX, rangeY+j] && Map.MonsterMap[rangeX-rearRange, rangeY + j])
                    {
                    GameSystem.AttackMonster(rangeX - rearRange, rangeY + j);
                        return true;
                    }
                    Console.SetCursorPosition(rangeX, rangeY + j);
                    Console.Write("#");

                    attackX.Add(rangeX);
                    attackY.Add(rangeY + j);
                }
            return false;
        }

        public void AttackByMonster(int monsterAttack)
        {
            if (currentHp > monsterAttack)
            {
                currentHp -= monsterAttack;
            }
            else
            {
                Die();
            }
        }

        public void Die()
        {
            GameSystem.monsterMove = null;
            currentHp=myTotalStatus.hp;
            OnVillage();
        }
        #endregion

        #region 캐릭터 초기 설정
        static Status InitBaseStatus()
        {
            Status baseStatus;
            baseStatus.defence = 0;
            baseStatus.attack = 1;
            baseStatus.hp = 10;

            return baseStatus;
        }

        public void InitBaseCharacter()
        {
            name = "모험가";

            //초기 스탯 설정
            myBaseStatus = InitBaseStatus();
            myTotalStatus = InitBaseStatus();

            //초기 인벤토리와 장비 설정
            inventory = new Inventory();

            InitBaseEquipment();
        } 

        //캐릭터 이름 설정
        public void SetMyName()
        {
            ConsoleKeyInfo key;
            bool setName = true;
            bool retry = false;
            string playerName;

            while (true)
            {
                setName = true;
                Console.WriteLine("▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽\n▷   당 신 은     누 구 신 가 요 ?  ◁\n△△△△△△△△△△△△△△△△△△△\n");
                Console.Write("이름 : ");
                playerName = Console.ReadLine();
                Console.WriteLine($"\n당신의 이름은 [{playerName}] 맞습니까?\n\n다음으로 넘어가고자 한다면 Enter\n재입력을 원하시면 Esc 키를 눌러주세요\n");
                while (setName)
                {
                    key = Console.ReadKey(true);

                    switch (key.Key)
                    {
                        case ConsoleKey.Enter:
                            setName = false;
                            retry = false;
                            break;
                        case ConsoleKey.Escape:
                            Console.Clear();
                            setName = false;
                            retry = true;
                            break;
                    }
                }
                if (!retry)
                {
                    break;
                }
            }
            name = playerName;
            Console.Clear();
        }
        //초기 시작시 혹은 레벨업 시(레벨업 시는 마을에서) 스테이터스 설정
        public void InitBaseEquipment()
        {
            equipment.myItem = new List<PlayerItem>();
            equipment.myPotion = new Potion("체력 포션", 50);
        }
        public void SetMyCharacterOnStart()
        {
            SetMyName();
            SetMyStatus();
        }

        #endregion

        #region 캐릭터 설정
        public void ShowMyCharacter()
        {
            Console.Clear();
            Console.WriteLine(" _   _  ___  __  __ _____ \r\n| | | |/ _ \\|  \\/  | ____|\r\n| |_| | | | | |\\/| |  _|  \r\n|  _  | |_| | |  | | |___ \r\n|_| |_|\\___/|_|  |_|_____|");
            Console.WriteLine("무엇을 확인하시겠습니까?");
            Console.WriteLine("0. 마을로 귀환");
            Console.WriteLine("1. 내 캐릭터의 스테이터스");
            Console.WriteLine("2. 내 캐릭터의 장비");
            Console.WriteLine("3. 인벤토리");
            Console.WriteLine("4. 레벨업");
            Console.WriteLine("5. 스탯 설정");
            Console.WriteLine("\n원하는 번호를 입력하세요\n");
            Console.Write("▶ ");
            switch (Read.ReadInt(0,5))
            {
                case 0:
                    OnVillage();
                    break;
                case 1:
                    ShowMyCharacterInformation();
                    break;
                case 2:
                    ShowMyEquipment();
                    break;
                case 3:
                    ShowMyInventory();
                    break;
                case 4:
                    LevelUp(); 
                    break;
                case 5:
                    SetMyStatus();
                    ShowMyCharacter();
                    break;
            }
        }
        void ShowMyCharacterInformation()
        {
            Console.Clear();
            ConsoleKeyInfo key;
            bool returnPage = true;

            Console.WriteLine($"이름 : {name}");
            Console.WriteLine($"\n▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽\n▷        B a s e S t a t u s        ◁\n△△△△△△△△△△△△△△△△△△△\n");
            Console.WriteLine($"HP : {myBaseStatus.hp}");
            Console.WriteLine($"공격력 : {myBaseStatus.attack}");
            Console.WriteLine($"방어력 : {myBaseStatus.defence}");

            Console.WriteLine($"\n▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽\n▷       T o t a l S t a t u s       ◁\n△△△△△△△△△△△△△△△△△△△\n");
            Console.WriteLine($"HP : {currentHp}/{myTotalStatus.hp}");
            Console.WriteLine($"공격력 : {myTotalStatus.attack}");
            Console.WriteLine($"방어력 : {myTotalStatus.defence}");

            Console.WriteLine("\nEnter키 입력시 캐릭터 정보 화면으로 복귀합니다.");
            while (returnPage)
            {
                key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        returnPage = false;
                        break;
                }
            }
            ShowMyCharacter();
        }
        //캐릭터 스탯 설정
        int SetMyStatusDetail(int minStatPoint)
        {
            int usingStatPoint = 0;
            while (true)
            {
                usingStatPoint = Read.ReadUInt(0);
                if (usingStatPoint > statPoint)
                {
                    Console.WriteLine("잔여스탯을 넘어섰습니다. 다시 입력해주세요");
                    Console.Write("▶ ");
                    continue;
                }
                if (minStatPoint > usingStatPoint)
                {
                    Console.WriteLine($"최소 {minStatPoint} point가 필요합니다.");
                    Console.Write("▶ ");
                    continue;
                }
                break;
            }
            statPoint -= usingStatPoint;
            return usingStatPoint;
        }
        void SetMyStatus()
        {
            bool setStatus = true;
            bool retry = false;
            ConsoleKeyInfo key;

            int originalStatPoint = statPoint;
            int originalHp = myBaseStatus.hp;
            int originalAttack = myBaseStatus.attack;
            int originalDefence = myBaseStatus.defence;

            if (originalStatPoint > 0)
            {
                //사용자 입력 기반 스탯 배분(hp는 최소 1 이상 필요)
                //입력 순서 hp 공격력, 방어력
                while (true)
                {
                    Console.WriteLine($"▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽\n▷ 스탯을 분배해주세요(총 {originalStatPoint} point) ◁\n△△△△△△△△△△△△△△△△△△△");

                    setStatus = true;
                    statPoint = originalStatPoint;
                    myBaseStatus.hp = originalHp;
                    myBaseStatus.attack = originalAttack;
                    myBaseStatus.defence = originalDefence;

                    Console.WriteLine($"\n▶ 잔여스탯 {statPoint: 000} point ◀");
                    Console.Write("HP(기본 10) : ");
                    myBaseStatus.hp += SetMyStatusDetail(0);
                    Console.WriteLine($"\n▶ 잔여스탯 {statPoint: 000} point ◀");
                    Console.Write("공격력(기본 1) : ");
                    myBaseStatus.attack += SetMyStatusDetail(0);

                    Console.WriteLine($"\n▶ 잔여스탯 {statPoint: 000} point ◀");
                    Console.Write("방어력(기본 0) : ");
                    myBaseStatus.defence += SetMyStatusDetail(0);

                    Console.WriteLine("\n스탯 분배에 만족한다면 Enter\n재설정을 원한다면 Esc 키를 눌러주세요");
                    while (setStatus)
                    {
                        key = Console.ReadKey(true);
                        switch (key.Key)
                        {
                            case ConsoleKey.Enter:
                                setStatus = false;
                                retry = false;
                                break;
                            case ConsoleKey.Escape:
                                Console.Clear();
                                setStatus = false;
                                retry = true;
                                break;
                        }
                    }
                    if (!retry)
                    {
                        break;
                    }
                }
                SetTotalStatus();
                Console.WriteLine();
            }
            else
            {
                bool returnPage = true;
                Console.WriteLine("잔여 스탯이 부족합니다.");
                Console.WriteLine("\nEnter키 입력시 캐릭터 정보 화면으로 복귀합니다.");
                while (returnPage)
                {
                    key = Console.ReadKey(true);

                    switch (key.Key)
                    {
                        case ConsoleKey.Enter:
                            returnPage = false;
                            break;
                    }
                }
                //ShowMyCharacter();
            }
        }

        //캐릭터 Total 스테이터스(초기 스탯+장비 스탯_
        public void SetTotalStatus()
        {
            //향후 해당 값들에 아이템 스탯 추가 예정
            int hp = 0;
            int defence = 0;
            int attack = 0;

            //아이템 수치만큼 추가
            for (int i = 0; i < equipment.myItem.Count; i++)
            {
                hp += equipment.myItem[i].hp;
                defence += equipment.myItem[i].defence;
                attack += equipment.myItem[i].attack;
            }

            myTotalStatus.hp = myBaseStatus.hp + hp;
            currentHp = myTotalStatus.hp;
            myTotalStatus.defence = myBaseStatus.defence + defence;
            myTotalStatus.attack = myBaseStatus.attack + attack;
        }
        #endregion
        
        //마을로 이동하는 메서드
        public void OnVillage()
        {

            GameSystem.RemoveMonster();
            GameSystem.NewMapMonster();
            currentStage = 0;
            onVillage = true;
            Map.InitBaseMap();
            Map.InitMonsterAttackMap();
            Map.MakePortal(0, 65, 25, 31, Map.adventurePortal_Stage1);
            Map.MakePortal(147, 182, 28, 34, Map.shopPortal);
            Map.MakePortal(81, 116, 52, 58, Map.homePortal);
            Map.MakePortal(0, 32, 54, 59,Map.villagePortal);


            Map.DrawVillageMap();
            pos.x = 100;
            pos.y = 30;
            SetCharacterPos();

            onAdventure = false;
        }

        public void OnAdventure()
        {
            onVillage = false;
            onAdventure = true;
            Map.DrawAdventureMap();
            switch (currentStage)
            {
                case 0:
                    Map.InitBaseMapStage1();
                    break;
                case 1:
                    Map.InitBaseMapStage2();
                    break;
                case 2:
                    Map.InitBaseMapStage3();
                    break;
            }

            Map.DrawBaseMap();
            currentStage++;

            Map.SetPlayerMap(pos.x, pos.y);
            pos.x = 40;
            pos.y = 57;
            SetCharacterPos();
            DrawMyInformation();
        }

        //상점으로 이동
        public void OnShop()
        {
            onVillage=false;
            onAdventure=false;
            Console.Clear();
            bool onShop = true;
            int myInt;
            Console.WriteLine(" ____  _   _  ___  ____  \r\n/ ___|| | | |/ _ \\|  _ \\ \r\n\\___ \\| |_| | | | | |_) |\r\n ___) |  _  | |_| |  __/ \r\n|____/|_| |_|\\___/|_|  ");
            Console.WriteLine("\n0. 마을로 귀환");
            Console.WriteLine("\n1. 목검 : 500 G");
            Console.WriteLine("\n2. 철검 : 800 G");
            Console.WriteLine("\n3. 바람의 검 : 2,000 G");
            Console.WriteLine("\n4. 용사의 검 : 10,000 G");
            Console.WriteLine("\n5. 중급 체력 포션 : 50 G");
            Console.WriteLine("\n현재 자본 : " + inventory.money);


            while (onShop)
            {
                Console.Write("\n▶ ");
                myInt = Read.ReadUInt(0);
                switch (myInt)
                {
                    case 0:
                        onShop = false;
                        break;
                    case 1:
                        if (inventory.money >= 500)
                        {
                            inventory.inventoryItem.Add(new PlayerItem("목검", 3, 0, 0, ItemType.Weapon));
                            inventory.money -= 500;
                            Console.WriteLine("[무기 : 목검]을 구매하였습니다.");
                            Console.WriteLine("현재 자본 : " + inventory.money + "\n");
                        }
                        break;
                    case 2:
                        if (inventory.money >= 800)
                        {
                            inventory.inventoryItem.Add(new PlayerItem("철검", 8, 0, 0, ItemType.Weapon));
                            inventory.money -= 800;
                            Console.WriteLine("[무기 : 철검]을 구매하였습니다.");
                            Console.WriteLine("현재 자본 : " + inventory.money + "\n");
                        }
                        break;
                    case 3:
                        if (inventory.money >= 2000)
                        {
                            inventory.inventoryItem.Add(new PlayerItem("바람의 검", 20, 10, 5, ItemType.Weapon));
                            inventory.money -= 2000;
                            Console.WriteLine("[무기 : 바람의 검]을 구매하였습니다.");
                            Console.WriteLine("현재 자본 : " + inventory.money + "\n");
                        }
                        break;
                    case 4:
                        if (inventory.money >= 10000)
                        {
                            inventory.inventoryItem.Add(new PlayerItem("용사의 검", 100, 50, 30, ItemType.Weapon));
                            inventory.money -= 10000;
                            Console.WriteLine("[무기 : 용사의 검]을 구매하였습니다.");
                            Console.WriteLine("현재 자본 : " + inventory.money + "\n");
                        }
                        break;
                    case 5:
                        if (inventory.money >= 50)
                        {
                            inventory.inventoryPotion.Add(new Potion("중급 체력 포션", 100));
                            inventory.money -= 50;
                            Console.WriteLine("[포션 : 중급 체력 포션]을 구매하였습니다.");
                            Console.WriteLine("현재 자본 : " + inventory.money + "\n");
                        }
                        break;
                }
            }

            OnVillage();
        }


        //인벤토리 내용물을 보여주는 메서드
        public void ShowMyInventory()
        {
            Console.Clear();
            ConsoleKeyInfo key;
            bool returnPage = true;

            Console.WriteLine($"\n▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽\n▷         I n v e n t o r y         ◁\n△△△△△△△△△△△△△△△△△△△\n");
            for (int i = 0; i < inventory.inventoryItem.Count; i++)
            {
                Console.WriteLine($"[{i + 1}]번 장비 인벤토리 ");
                if (inventory.inventoryItem[i].name == "")
                {
                    Console.WriteLine($"비어있습니다\n");
                }
                else
                {
                    Console.WriteLine($"이름 : {inventory.inventoryItem[i].name}\n");
                }

            }

            for (int i = 0; i < inventory.inventoryPotion.Count; i++)
            {
                Console.WriteLine($"[{i + 1}]번 포션 인벤토리");
                if (inventory.inventoryPotion[i].name == "")
                {
                    Console.WriteLine("비어있습니다\n");
                }
                else
                {
                    Console.WriteLine($"이름 : {inventory.inventoryPotion[i].name}\n");
                }

            }
            while (returnPage)
            {
                Console.WriteLine("0. 캐릭터 정보 화면으로 복귀");
                Console.WriteLine("1. 장비 장착");
                Console.WriteLine("2. 포션 장착");
                Console.WriteLine("원하는 동작을 선택하세요.");
                Console.Write("\n▶ ");
                switch (Read.ReadInt(0, 2))
                {
                    case 0:
                        returnPage = false;
                        break;
                    case 1:
                        Console.WriteLine("착용을 원하는 장비 번호를 입력하세요");
                        Console.Write("\n▶ ");
                        EquipItem(Read.ReadUInt(0)-1);
                        break;
                    case 2:
                        Console.WriteLine("착용을 원하는 포션 번호를 입력하세요");
                        Console.Write("\n▶ ");
                        EquipPotion(Read.ReadUInt(0) - 1);
                        break;
                }
            }
            ShowMyCharacter();
        }

        //아이템을 장착하는 메서드
        public void EquipItem(int itemNum)
        {
            if (equipment.myItem.Count < MaxEquipment)
            {
                if (itemNum<inventory.inventoryItem.Count)
                {
                    equipment.myItem.Add(inventory.inventoryItem[itemNum]);
                    inventory.inventoryItem.RemoveAt(itemNum);
                    Console.WriteLine($"[{equipment.myItem[equipment.myItem.Count - 1].name}]을 장비하였습니다.");
                    SetTotalStatus();
                }
                else
                {
                    Console.WriteLine("해당 인벤토리에 장비가 없습니다.\n");
                }
            }
            else
            {
                Console.WriteLine("더 이상 장비를 착용할 수 없습니다.\n");
            }
        }
        //포션을 장착하는 메서드
        public void EquipPotion(int itemNum)
        {
            if (itemNum < inventory.inventoryPotion.Count)
            {
                if (inventory.inventoryPotion[itemNum] != null)
                {
                    if (equipment.myPotion == null)
                    {
                        equipment.myPotion = inventory.inventoryPotion[itemNum];
                        inventory.inventoryPotion.RemoveAt(itemNum);
                        Console.WriteLine($"[{equipment.myPotion.name}]을 장비하였습니다.\n");
                    }
                    else
                    {
                        inventory.inventoryPotion.Add(equipment.myPotion);
                        equipment.myPotion = inventory.inventoryPotion[itemNum];
                        inventory.inventoryPotion.RemoveAt(itemNum);
                        Console.WriteLine($"[{equipment.myPotion.name}]을 장비하였습니다.\n");
                    }
                }
            }
            else
            {
                Console.WriteLine("해당 인벤토리에 포션이 없습니다.\n");
            }
        }
        //착용중인 장비를 보여주는 메서드
        public void ShowMyEquipment()
        {
            Console.Clear();
            bool returnPage = true;

            Console.WriteLine($"\n▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽\n▷         E q u i p m e n t        ◁\n△△△△△△△△△△△△△△△△△△△\n");
            for (int i = 0; i < equipment.myItem.Count; i++)
            {
                Console.WriteLine($"[{i + 1}]번 장비 ");
                if (equipment.myItem[i]== null)
                {
                    Console.WriteLine($"비어있습니다\n");
                }
                else
                {
                    Console.WriteLine($"이름 : {equipment.myItem[i].name}\n");
                }
            }

            Console.WriteLine("[포션 주머니]");
            if (equipment.myPotion== null)
            {
                Console.WriteLine($"비어있습니다\n");
            }
            else
            {
                Console.WriteLine($"이름 : {equipment.myPotion.name}\n");
            }


            while (returnPage)
            {
            Console.WriteLine("0. 캐릭터 정보 화면으로 복귀");
            Console.WriteLine("1. 장비 해제");
            Console.WriteLine("2. 포션 해제");
                Console.WriteLine("원하는 동작을 선택하세요.");
                Console.Write("\n▶ ");
                switch (Read.ReadInt(0,2))
                {
                    case 0:
                        returnPage = false;
                        break;
                    case 1:
                        Console.WriteLine("착용 해제를 원하는 장비 번호를 입력하세요");
                        Console.Write("\n▶ ");
                        UnEquipItem(Read.ReadUInt(0));
                        break;
                    case 2:
                        UnEquipPotion();
                        break;
                }
            }

            ShowMyCharacter();
        }

        //아이템을 장착하는 메서드
        public void UnEquipItem(int itemNum)
        {
            if (itemNum< inventory.inventoryItem.Count)
            {
                inventory.inventoryItem.Add(equipment.myItem[itemNum]);
                Console.WriteLine($"[{equipment.myItem[itemNum].name}]을 장비 해제하였습니다.\n");
                equipment.myItem.RemoveAt(itemNum);
                SetTotalStatus();
            }
            else
            {
                Console.WriteLine("해당 위치에 장비를 착용하고있지 않습니다.\n");
            }
        }
        public void UnEquipPotion()
        {
            if (equipment.myPotion!=null)
            {
                inventory.inventoryPotion.Add(equipment.myPotion);
                Console.WriteLine($"[{equipment.myPotion.name}]을 장비 해제하였습니다.\n");
                equipment.myPotion = null;
            }
            else
            {
                Console.WriteLine("포션을 착용하고있지 않습니다.\n");
            }
        }

        public void onGame()
        {
            Console.Clear();
            Map.SetMap();
            statPoint = 50;
            level = 1;
            SetMaxExp();
            //캐릭터 초기 설정
            InitBaseCharacter();

            SetMyCharacterOnStart();

            Console.Clear();

            //Control();
            onFront = true;
            OnVillage();
        }

        public void IncreaseExp(Monster monster)
        {
            exp+=monster.exp;
        }
        public void SetMaxExp()
        {
            maxExp = level * 10;
        }

        public void LevelUp()
        {
            bool returnPage=true;
            ConsoleKeyInfo key;
            int levelUpCount = 0;
            if (exp >= maxExp)
            {
                while (exp >= maxExp)
                {
                    level++;
                    exp -= maxExp;
                    SetMaxExp();
                    statPoint += 5;
                    levelUpCount++;
                }
                Console.WriteLine($"레벨이 총 [{levelUpCount}] 상승하였습니다.");
            }
            else
            {
                Console.WriteLine("경험치가 부족합니다.");
                Console.WriteLine($"{exp}/{maxExp}");
            }
            Console.WriteLine("\nEnter키 입력시 캐릭터 정보 화면으로 복귀합니다.");
            while (returnPage)
            {
                key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        returnPage = false;
                        break;
                }
            }
            ShowMyCharacter();
        }

        public void DrawMyInformation()
        {
            string myHP = currentHp + " / " + myTotalStatus.hp;
            string myAttack = ""+myTotalStatus.attack;
            string myDefence = ""+myTotalStatus.defence;
            Console.SetCursorPosition(2, 1);
            Console.Write("▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽");
            Console.SetCursorPosition(2, 2);
            Console.Write("▷       I n f o r m a t i o n      ◁");
            Console.SetCursorPosition(2, 3);
            Console.Write($"▷  HP  : "+myHP);
            DrawSpaceBar(myHP);
            Console.SetCursorPosition(2, 4);
            Console.Write($"▷  Att : "+myAttack);
            DrawSpaceBar(myAttack);
            Console.SetCursorPosition(2, 5);
            Console.Write($"▷  Def : "+myDefence);
            DrawSpaceBar(myDefence);


            Console.SetCursorPosition(2, 6);
            Console.Write("△△△△△△△△△△△△△△△△△△△");
        }
        public void DrawSpaceBar(string s)
        {
            for (int i = 0; i < 26 - s.Length; i++)
            {
                Console.Write(" ");
            }
            Console.Write("◁");
        }
    }
}
