using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleProject_2
{
    enum Direction
    { left, right, up, down }

    struct MyPos
    {
        public int x;
        public int y;
    }

    // 캐릭터 스테이터스, hp, mp, 방어력, 공격력, 치명타 확률, 치명타 데미지
    struct Status
    {
        public int hp;
        //public int mp;
        public int defence;
        public int attack;
    }

    //플레이어가 착용한 아이템
    struct Equipment
    {
        public PlayerItem[] myItem;
        public Potion myPotion;
    }


    internal class Player
    {
        string name;
        Direction dir;

        static string[] playerImage;
        static string[] eraserplayerImage;

        bool onAdventure;
        bool onVillage;
        //더블점프 포기하고 그냥 점프중엔 다시 점프 조작 안되게 만들기 위함
        bool onJump;

        //전방을 바라보고 있는지 확인
        bool onFront;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        //해당 좌표는 플레이어의 좌측 상단 좌표이다
        public MyPos pos;


        //캐릭터 자체의 스탯
        public Status myBaseStatus;

        //캐릭터 자체의 스탯 + 아이템 스탯<-향후 아이템 추가 후 메서드 추가
        public Status myTotalStatus;

        //인벤토리
        public Inventory inventory;

        //착용 장비
        Equipment equipment;

        //경험치 및 레벨
        int level;
        int maxExp;
        public int currentExp;
        int statPoint;
        public Player(bool test)
        {
        }

        public Player()
        {
            playerImage = new string[2];
            playerImage[0] = " ∧ ∧";
            playerImage[1] = "( '▽' )";//<-이거 특수문자 때문에 크기 8 먹는다 [2]*8 배열인듯
            Name = "Nothing";
            eraserplayerImage = new string[2];
            eraserplayerImage[0] = "        ";
            eraserplayerImage[1] = "        ";
            pos.x = 100;
            pos.y = 30;

        }



        #region 조작
        public void ControlPlayer()
        {
            BackgroundWorker control = new BackgroundWorker();
            ConsoleKeyInfo key;
            control.DoWork += (sender, e) =>
            {
                Stopwatch watch= new Stopwatch();
                watch.Start();
                while (true)
                {
                    if (Console.KeyAvailable)
                    {
                        if (onVillage || onAdventure)
                        {
                            key = Console.ReadKey(true);
                            switch (key.Key)
                            {
                                case ConsoleKey.UpArrow:
                                    if (!onAdventure && onVillage)
                                    {
                                        MoveUp();
                                    }
                                    else if (onAdventure && !onVillage)
                                    {
                                        //Console.WriteLine("점프!!");
                                        MoveUp();
                                        MoveUp();
                                        MoveUp();
                                        MoveUp();
                                        MoveUp();
                                        MoveUp();
                                        MoveUp();
                                        MoveUp();
                                        MoveUp();
                                        MoveUp();
                                        //Console.WriteLine("점프 성공");
                                    }
                                    break;
                                case ConsoleKey.DownArrow:
                                    MoveDown();
                                    break;
                                case ConsoleKey.LeftArrow:
                                    onFront = false;
                                    MoveLeft();
                                    break;
                                case ConsoleKey.RightArrow:
                                    onFront = true;
                                    MoveRight();
                                    break;
                                case ConsoleKey.A:
                                    if (onFront)
                                    {
                                        Attack(3, 0, 0, 0);
                                    }
                                    else
                                    {
                                        Attack(0, 3, 0, 0);
                                    }
                                    break;
                                case ConsoleKey.Spacebar:
                                    if (Map.shopPortal[pos.x, pos.y])
                                    {
                                        onVillage = false;
                                        onAdventure = false;
                                        OnShop();
                                    }
                                    if (Map.adventurePortal[pos.x, pos.y])
                                    {
                                        OnAdventure();
                                        Console.WriteLine("이동했다ㅏㅏㅏㅏㅏ");
                                    }
                                    if (Map.homePortal[pos.x, pos.y])
                                    {
                                        onVillage = false;
                                        onAdventure = false;
                                        ShowMyCharacter();
                                    }
                                    if (Map.villagePortal[pos.x, pos.y])
                                    {
                                        OnVillage();
                                    }
                                    break;
                            }
                        }
                    }
                    if (onAdventure && !onVillage)
                    {
                        if (!Map.BaseMap[pos.x, pos.y + playerImage.Length]&&watch.ElapsedTicks%5000==0)
                        {
                            Move(Direction.down);
                            watch.Restart();
                            //여기다가 몬스터 일정 시간마다 움직이는 코드 넣어도 될 듯?
                        }
                    }
                }
            };
            control.RunWorkerCompleted += (sender, e) =>
            {
                control = null;
            };
            control.RunWorkerAsync();
        }
        public void SetCharacterPos(string[] s)
        {
            Map.SetPlayerMap(pos.x, pos.y);
            Console.SetCursorPosition(pos.x, pos.y);
            Console.Write(s[0]);
            Console.SetCursorPosition(pos.x, pos.y + 1);
            Console.Write(s[1]);
        }

        public void Move(Direction dir)
        {
            Map.SetPlayerMap(pos.x,pos.y);
            if (onVillage)
            {
                Console.SetCursorPosition(pos.x, pos.y);
                for (int i = 0; i < 8; i++)
                {
                    Console.Write(Map.villageMap[pos.x + i, pos.y]);
                }
                Console.SetCursorPosition(pos.x, pos.y+1);
                for (int i = 0; i < 8; i++)
                {
                    Console.Write(Map.villageMap[pos.x + i, pos.y+1]);
                }
            }
            else if (onAdventure)
            {
                Console.SetCursorPosition(pos.x, pos.y);
                for (int i = 0; i < 8; i++)
                {
                    Console.Write(Map.adventureMap[pos.x + i, pos.y]);
                }
                Console.SetCursorPosition(pos.x, pos.y+1);
                for (int i = 0; i < 8; i++)
                {
                    Console.Write(Map.adventureMap[pos.x + i, pos.y+1]);
                }
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

            SetCharacterPos(playerImage);
        }
        public void MoveLeft()
        {
            //특수문자라 -2다 일반이면 -1로 바꿔야 한다
            if (!Map.BaseMap[pos.x - 2, pos.y])
            {
                Move(Direction.left);
            }
        }
        public void MoveRight()
        {
            if (!Map.BaseMap[pos.x + playerImage[1].Length + 1, pos.y])
            {
                Move(Direction.right);
            }
        }
        public void MoveUp()
        {
            if (!Map.BaseMap[pos.x, pos.y - playerImage.Length + 1])
            {
                Move(Direction.up);
            }

        }
        public void MoveDown()
        {
            //왜 이런 값이 나올까 생각해봤는제 pos는 플레이어의 좌측 상단 좌표
            // pos.y+playerImage+1이 아닌 이유는 애초에 내가 아랫줄 맵은 baseMap.GetLength(1)-1d으로 받아놔서 그런거 같음
            if (!Map.BaseMap[pos.x, pos.y + playerImage.Length])
            {
                Move(Direction.down);
            }

        }
        public void Attack(int forwardRangeX, int rearRangeX, int topRangeY, int bottomRangeY)
        {
            //Map.SetAttackMap(pos,forwardRangeX, rearRangeX, topRangeY, bottomRangeY, 1);
            SetAttackMap(pos, forwardRangeX, rearRangeX, topRangeY, bottomRangeY, 1);
        }
        public static void SetAttack(int x, int y, List<int> attackRangeX, List<int> attackRangeY)
        {
            Map.AttackMap[x, y] = true;
            attackRangeX.Add(x);
            attackRangeY.Add(y);
            Console.SetCursorPosition(x, y);
            Console.Write("c");
            if (Map.AttackMap[x, y] && Map.MonsterMap[x, y])
            {
                Console.WriteLine("몬스터 공겨어어어억");
                GameSystem.AttackMonster(x,y);
                //Map.MonsterMap[x, y] = false;
            }
        }

        //이거 호출하면 해당 좌표가 true로 바뀌고 지속시간 끝나면 다시 false로 바뀐다
        //이거도 시간 나면 통합 예정
        public static void SetAttackMap(MyPos pos, int forwardRangeX, int rearRangeX, int topRangeY, int bottomRangeY, int duration)
        {
            BackgroundWorker attack = new BackgroundWorker();
            attack.DoWork += (sender, e) =>
            {
                int attackX;
                int attackY;
                List<int> attackRangeX = new List<int>();
                List<int> attackRangeY = new List<int>();
                for (int i = 1; i <= forwardRangeX; i++)
                {
                    //pos.x + playerImage[1].Length
                    attackX = pos.x + playerImage[1].Length + i;
                    attackY = pos.y;
                    if (attackX < 200)
                    {
                        SetAttack(attackX, attackY, attackRangeX, attackRangeY);
                        SetAttack(attackX, attackY + 1, attackRangeX, attackRangeY);
                    }
                    else
                    {
                        break;
                    }
                }
                for (int i = 1; i <= rearRangeX; i++)
                {
                    attackX = pos.x - i;
                    attackY = pos.y;
                    if (attackX >= 0)
                    {
                        SetAttack(attackX, attackY, attackRangeX, attackRangeY);
                        SetAttack(attackX, attackY + 1, attackRangeX, attackRangeY);
                    }
                    else
                    {
                        break;
                    }
                }
                for (int i = 1; i <= topRangeY; i++)
                {
                    attackX = pos.x;
                    attackY = pos.y - i;
                    if (attackY >= 0)
                    {
                        for (int j = 0; j < playerImage[1].Length + 1; j++)
                        {
                            SetAttack(attackX + j, attackY, attackRangeX, attackRangeY);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                for (int i = 1; i <= bottomRangeY; i++)
                {
                    attackX = pos.x;
                    attackY = pos.y + i + playerImage.Length - 1;
                    if (attackY <= 60)
                    {

                        for (int j = 0; j < playerImage[1].Length + 1; j++)
                        {
                            SetAttack(attackX + j, attackY, attackRangeX, attackRangeY);
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                Thread.Sleep(duration * 1000);

                for (int i = 0; i < attackRangeX.Count; i++)
                {
                    Console.SetCursorPosition(attackRangeX[i], attackRangeY[i]);
                    Console.Write(" ");
                    Map.AttackMap[attackRangeX[i], attackRangeY[i]] = false;
                }
                attackRangeX = null;
                attackRangeY = null;
            };
            attack.RunWorkerCompleted += (sender, e) =>
            {
                attack = null;
            };
            attack.RunWorkerAsync();
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
            string name;

            while (true)
            {
                setName = true;
                Console.WriteLine("▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽\n▷   당 신 은     누 구 신 가 요 ?   ◁\n△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△\n");
                Console.Write("이름 : ");
                name = Console.ReadLine();
                Console.WriteLine($"\n당신의 이름은 [{name}] 맞습니까?\n\n다음으로 넘어가고자 한다면 Enter\n재입력을 원하시면 A 키를 눌러주세요\n");
                while (setName)
                {
                    key = Console.ReadKey(true);

                    switch (key.Key)
                    {
                        case ConsoleKey.Enter:
                            setName = false;
                            retry = false;
                            break;
                        case ConsoleKey.A:
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
            Name = name;
            Console.Clear();
        }
        //초기 시작시 혹은 레벨업 시(레벨업 시는 마을에서) 스테이터스 설정
        public void SetMyStatusOnStart()
        {
            bool setStatus = true;
            bool retry = false;
            ConsoleKeyInfo key;

            int originalStatPoint = statPoint;
            int originalHp = myBaseStatus.hp;
            int originalAttack = myBaseStatus.attack;
            int originalDefence = myBaseStatus.defence;

            //사용자 입력 기반 스탯 배분(hp는 최소 1 이상 필요)
            //입력 순서 hp mp 공격력, 방어력 크확 ,크뎀
            while (true)
            {
                Console.WriteLine($"▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽\n▷ 스탯을 분배해주세요(총 {statPoint} point) ◁\n△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△");

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
                myBaseStatus.attack += SetMyStatusDetail(1);

                Console.WriteLine($"\n▶ 잔여스탯 {statPoint: 000} point ◀");
                Console.Write("방어력(기본 0) : ");
                myBaseStatus.defence += SetMyStatusDetail(1);

                Console.WriteLine("\n스탯 분배에 만족한다면 Enter\n재설정을 원한다면 A 키를 눌러주세요");
                while (setStatus)
                {
                    key = Console.ReadKey(true);
                    switch (key.Key)
                    {
                        case ConsoleKey.Enter:
                            setStatus = false;
                            retry = false;
                            break;
                        case ConsoleKey.A:
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
        public void InitBaseEquipment()
        {
            equipment.myItem = new PlayerItem[5];
            for (int i = 0; i < equipment.myItem.Length; i++)
            {
                equipment.myItem[i] = new PlayerItem();
            }
            equipment.myPotion = new Potion();
        }
        public void SetMyCharacterOnStart()
        {
            SetMyName();
            SetMyStatusOnStart();
        }

        #endregion

        #region 캐릭터 설정
        public void ShowMyCharacter()
        {
            Console.Clear();
            int myInt;
            Console.WriteLine($"▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽\n▷     캐 릭 터           정 보      ◁\n△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△\n");
            Console.WriteLine("무엇을 확인하시겠습니까?");
            Console.WriteLine("0. 내 캐릭터의 스테이터스");
            Console.WriteLine("1. 내 캐릭터의 장비");
            Console.WriteLine("2. 인벤토리");
            Console.WriteLine("3. 레벨업");
            Console.WriteLine("그 이외의 숫자 입력시 마을로 복귀합니다.");
            Console.WriteLine("\n원하는 번호를 입력하세요\n");
            Console.Write("▶ ");
            myInt = Read.ReadUInt(0);
            if (myInt > 3)
            {
                OnVillage();
            }
            else if (myInt == 0)
            {
                ShowMyCharacterInformation();
            }
            else if (myInt == 1)
            {
                ShowMyEquipment();
            }
            else if (myInt == 2)
            {
                ShowMyInventory();
            }
            else if (myInt == 3)
            {
                LevelUp();
            }
        }
        public void ShowMyCharacterInformation()
        {
            Console.Clear();
            ConsoleKeyInfo key;
            bool returnPage = true;

            Console.WriteLine($"이름 : {name}");
            Console.WriteLine($"\n▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽\n▷        B a s e S t a t u s        ◁\n△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△\n");
            Console.WriteLine($"HP : {myBaseStatus.hp}");
            Console.WriteLine($"공격력 : {myBaseStatus.attack}");
            Console.WriteLine($"방어력 : {myBaseStatus.defence}");

            Console.WriteLine($"\n▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽\n▷       T o t a l S t a t u s       ◁\n△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△\n");
            Console.WriteLine($"HP : {myTotalStatus.hp}");
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
        public int SetMyStatusDetail(int minStatPoint)
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

        //캐릭터 Total 스테이터스(초기 스탯+장비 스탯_
        public void SetTotalStatus()
        {
            //향후 해당 값들에 아이템 스탯 추가 예정
            int hp = 0;
            int defence = 0;
            int attack = 0;



            //아이템 수치만큼 추가
            for (int i = 0; i < equipment.myItem.Length; i++)
            {
                hp += equipment.myItem[i].hp;
                defence += equipment.myItem[i].defence;
                attack += equipment.myItem[i].attack;

            }

            myTotalStatus.hp = myBaseStatus.hp + hp;
            myTotalStatus.defence = myBaseStatus.defence + defence;
            myTotalStatus.attack = myBaseStatus.attack + attack;
        }
        #endregion
        
        //마을로 이동하는 메서드
        public void OnVillage()
        {
            onVillage = true;
            Map.InitBaseMap();

            Map.MakePortal(0, 65, 25, 31, Map.adventurePortal);
            Map.MakePortal(147, 182, 28, 34, Map.shopPortal);
            Map.MakePortal(81, 116, 52, 58, Map.homePortal);
            Map.MakePortal(0, 20, 49, 59,Map.villagePortal);

            GameSystem.NewMapMonster();

            //Console.Clear();
            Map.DrawVillageMap();
            SetCharacterPos(playerImage);

            //일단 테스트용이라 현재는 true로 둔거고
            //나중엔 사냥터 갔을 때만 true로 바꿀 예정
            onAdventure = false;
            //이거도 테스트용이라 지금 여기서 실행하는 거임
            //Gravity();

            //일단 지금 true로 설정해도 나중에 벽에 닿으면 false로 바뀜
            onJump = true;
        }

        //모험으로 이동
        public void OnAdventure()
        {
            onVillage=false;
            onAdventure=true;
            Map.DrawAdventureMap();
            Map.InitBaseMapStage1();
            Map.DrawBaseMap();
            SetCharacterPos(playerImage);
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
            Console.WriteLine("\n5. 체력 포션 : 50 G");
            Console.WriteLine("\n현재 자본 : " + inventory.money);


            while (onShop)
            {
                Console.Write("▶ ");
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
                            inventory.inventoryPotion.Add(new Potion("체력 포션", 50));
                            inventory.money -= 50;
                            Console.WriteLine("[포션 : 체력 포션]을 구매하였습니다.");
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

            Console.WriteLine($"\n▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽\n▷         I n v e n t o r y         ◁\n△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△\n");
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

        //착용중인 장비를 보여주는 메서드
        public void ShowMyEquipment()
        {

            Console.Clear();
            ConsoleKeyInfo key;
            bool returnPage = true;

            Console.WriteLine($"\n▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽▽\n▷         E q u i p m e n t         ◁\n△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△△\n");
            for (int i = 0; i < equipment.myItem.Length; i++)
            {
                Console.WriteLine($"[{i + 1}]번 장비 ");
                if (equipment.myItem[i].name == "")
                {
                    Console.WriteLine($"비어있습니다\n");
                }
                else
                {
                    Console.WriteLine($"이름 : {equipment.myItem[i].name}\n");
                }
            }


            Console.WriteLine("[포션 주머니]");
            if (equipment.myPotion.name == "")
            {
                Console.WriteLine($"비어있습니다\n");
            }
            else
            {
                Console.WriteLine($"이름 : {equipment.myPotion.name}\n");
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

            ControlPlayer();
            onFront = true;
            OnVillage();
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
            if (currentExp >= maxExp)
            {
                while (currentExp >= maxExp)
                {
                    level++;
                    currentExp -= maxExp;
                    SetMaxExp();
                    statPoint += 5;
                    levelUpCount++;
                }

                Console.WriteLine($"레벨이 총 [{levelUpCount}] 상승하였습니다.");

            }
            else
            {
                Console.WriteLine("경험치가 부족합니다.");
                Console.WriteLine($"{currentExp}/{maxExp}");
            }

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
    }
}
